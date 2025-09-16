#!/usr/bin/env python3
"""
Notion Content Downloader
Downloads Notion documentation, templates, and maps them to Unity scripts for manual analysis
"""

import os
import re
import json
import requests
import time
from pathlib import Path
from typing import Dict, List, Optional

class NotionDownloader:
    def __init__(self, notion_token: str):
        self.notion_token = notion_token
        self.base_url = "https://api.notion.com/v1"
        self.headers = {
            "Authorization": f"Bearer {notion_token}",
            "Content-Type": "application/json",
            "Notion-Version": "2022-06-28"
        }

    def get_page_content(self, page_id: str) -> Dict:
        """Get full content of a Notion page"""
        try:
            # Get page properties
            page_response = requests.get(
                f"{self.base_url}/pages/{page_id}",
                headers=self.headers
            )

            if page_response.status_code != 200:
                return {}

            page_data = page_response.json()

            # Get page blocks/content
            blocks_response = requests.get(
                f"{self.base_url}/blocks/{page_id}/children?page_size=100",
                headers=self.headers
            )

            if blocks_response.status_code != 200:
                return {}

            blocks = blocks_response.json().get('results', [])

            # Extract text content from blocks
            content_lines = []
            for block in blocks:
                block_text = self.extract_text_from_block(block)
                if block_text:
                    content_lines.append(block_text)

            title = self.extract_title_from_page(page_data)

            return {
                'title': title,
                'content': '\n'.join(content_lines),
                'raw_blocks': blocks
            }

        except Exception as e:
            print(f"❌ Chyba při čtení stránky {page_id}: {e}")
            return {}

    def extract_text_from_block(self, block: Dict) -> str:
        """Extract text from a Notion block"""
        try:
            block_type = block.get('type', '')

            if block_type in ['paragraph']:
                rich_text = block.get(block_type, {}).get('rich_text', [])
                return ''.join([text.get('plain_text', '') for text in rich_text])

            elif block_type in ['heading_1', 'heading_2', 'heading_3']:
                rich_text = block.get(block_type, {}).get('rich_text', [])
                level = block_type[-1]
                text = ''.join([text.get('plain_text', '') for text in rich_text])
                return f"{'#' * int(level)} {text}"

            elif block_type == 'bulleted_list_item':
                rich_text = block.get('bulleted_list_item', {}).get('rich_text', [])
                return '• ' + ''.join([text.get('plain_text', '') for text in rich_text])

            elif block_type == 'numbered_list_item':
                rich_text = block.get('numbered_list_item', {}).get('rich_text', [])
                return '1. ' + ''.join([text.get('plain_text', '') for text in rich_text])

            elif block_type == 'code':
                rich_text = block.get('code', {}).get('rich_text', [])
                language = block.get('code', {}).get('language', '')
                code_text = ''.join([text.get('plain_text', '') for text in rich_text])
                return f"```{language}\n{code_text}\n```"

            elif block_type == 'quote':
                rich_text = block.get('quote', {}).get('rich_text', [])
                return '> ' + ''.join([text.get('plain_text', '') for text in rich_text])

        except:
            pass

        return ""

    def extract_title_from_page(self, page_data: Dict) -> str:
        """Extract title from page data"""
        try:
            properties = page_data.get('properties', {})
            title_prop = properties.get('title', {})
            if title_prop.get('title'):
                return ''.join([text.get('plain_text', '') for text in title_prop['title']])
        except:
            pass
        return ""

    def safe_filename(self, name: str) -> str:
        """Convert name to safe filename"""
        # Remove invalid characters and replace with underscore
        safe_name = re.sub(r'[<>:"/\\|?*]', '_', name)
        safe_name = re.sub(r'[📁📄]', '', safe_name).strip()
        return safe_name

    def download_templates(self, templates_id: str, output_dir: str):
        """Download all templates"""
        templates_dir = os.path.join(output_dir, "templates")
        os.makedirs(templates_dir, exist_ok=True)

        def process_children(page_id: str):
            try:
                response = requests.get(
                    f"{self.base_url}/blocks/{page_id}/children?page_size=100",
                    headers=self.headers
                )

                if response.status_code != 200:
                    return

                children = response.json().get('results', [])

                for child in children:
                    if child['type'] == 'child_page':
                        title = child['child_page']['title']
                        child_id = child['id']

                        # Download templates (start with _Template)
                        if title.startswith('_Template'):
                            print(f"📋 Stahuji template: {title}")
                            content = self.get_page_content(child_id)

                            if content:
                                filename = self.safe_filename(title) + ".md"
                                filepath = os.path.join(templates_dir, filename)

                                with open(filepath, 'w', encoding='utf-8') as f:
                                    f.write(f"# {content['title']}\n\n")
                                    f.write(content['content'])

                                print(f"   ✅ Uložen: {filename}")

                time.sleep(0.3)

            except Exception as e:
                print(f"❌ Chyba při stahování templates: {e}")

        process_children(templates_id)

    def download_documentation(self, veverka_id: str, output_dir: str):
        """Download all documentation"""
        docs_dir = os.path.join(output_dir, "documentation")
        os.makedirs(docs_dir, exist_ok=True)

        def process_children(page_id: str, path: str = ""):
            try:
                response = requests.get(
                    f"{self.base_url}/blocks/{page_id}/children?page_size=100",
                    headers=self.headers
                )

                if response.status_code != 200:
                    return

                children = response.json().get('results', [])

                for child in children:
                    if child['type'] == 'child_page':
                        title = child['child_page']['title']
                        child_id = child['id']

                        # Skip folder pages (📁)
                        if title.startswith('📁'):
                            folder_name = title.replace('📁 ', '')
                            new_path = f"{path}/{folder_name}" if path else folder_name

                            # Create folder structure
                            folder_path = os.path.join(docs_dir, new_path)
                            os.makedirs(folder_path, exist_ok=True)

                            process_children(child_id, new_path)

                        elif title.startswith('📄') or (not title.startswith('_') and title not in ['Changelogs', 'Introduction', 'Software']):
                            # Documentation page
                            script_name = title.replace('📄 ', '')
                            print(f"📄 Stahuji dokumentaci: {script_name}")

                            content = self.get_page_content(child_id)

                            if content:
                                filename = self.safe_filename(script_name) + ".md"

                                # Save in appropriate folder
                                if path:
                                    filepath = os.path.join(docs_dir, path, filename)
                                else:
                                    filepath = os.path.join(docs_dir, filename)

                                with open(filepath, 'w', encoding='utf-8') as f:
                                    f.write(f"# {content['title']}\n\n")
                                    f.write(f"**Path:** {path}/{script_name}\n\n" if path else f"**Path:** {script_name}\n\n")
                                    f.write(content['content'])

                                print(f"   ✅ Uložen: {filename}")

                time.sleep(0.3)

            except Exception as e:
                print(f"❌ Chyba při stahování dokumentace: {e}")

        process_children(veverka_id)

    def copy_unity_scripts(self, output_dir: str):
        """Copy relevant Unity scripts to output directory"""
        scripts_dir = os.path.join(output_dir, "unity_scripts")
        os.makedirs(scripts_dir, exist_ok=True)

        script_count = 0

        for root, dirs, files in os.walk("./Assets"):
            # Skip certain directories
            if any(skip_dir in root for skip_dir in ['Editor', 'Tests', 'Test', 'TextMesh Pro']):
                continue

            for file in files:
                if file.endswith('.cs'):
                    source_path = os.path.join(root, file)
                    rel_path = os.path.relpath(source_path, './Assets')

                    # Create folder structure
                    target_folder = os.path.join(scripts_dir, os.path.dirname(rel_path))
                    os.makedirs(target_folder, exist_ok=True)

                    # Copy file
                    target_path = os.path.join(target_folder, file)

                    try:
                        with open(source_path, 'r', encoding='utf-8', errors='ignore') as f:
                            content = f.read()

                        with open(target_path, 'w', encoding='utf-8') as f:
                            f.write(content)

                        script_count += 1

                    except Exception as e:
                        print(f"⚠️ Chyba při kopírování {source_path}: {e}")

        print(f"📂 Zkopírováno {script_count} Unity scriptů")

    def create_mapping_file(self, output_dir: str):
        """Create a mapping file to help with comparisons"""
        mapping = {
            "folders": {
                "templates": "Šablony pro dokumentaci",
                "documentation": "Existující dokumentace z Notion",
                "unity_scripts": "Unity C# scripty z projektu"
            },
            "instructions": [
                "1. Porovnej dokumentaci s Unity scriptem stejného názvu",
                "2. Porovnej dokumentaci s odpovídajícím template",
                "3. Hledej:",
                "   - Chybějící informace v dokumentaci",
                "   - Rozdíly oproti template struktuře",
                "   - Neaktuální informace",
                "   - Správnost popisu funkcí"
            ]
        }

        mapping_file = os.path.join(output_dir, "MAPPING_AND_INSTRUCTIONS.json")
        with open(mapping_file, 'w', encoding='utf-8') as f:
            json.dump(mapping, f, ensure_ascii=False, indent=2)

        print(f"📋 Vytvořen mapping file: MAPPING_AND_INSTRUCTIONS.json")

def main():
    NOTION_TOKEN = "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"
    VEVERKA_ID = "20547dcf01688077a81cf9a7d7d9fed5"
    TEMPLATES_ID = "21f47dcf016880809142ec53df8b2c18"

    # Create output directory in temp
    output_dir = "./temp_notion_analysis"
    os.makedirs(output_dir, exist_ok=True)

    print("🚀 Stahuji veškerý obsah z Notion...")

    downloader = NotionDownloader(NOTION_TOKEN)

    try:
        # Download everything
        print("\n📋 Stahuji Templates...")
        downloader.download_templates(TEMPLATES_ID, output_dir)

        print("\n📄 Stahuji dokumentaci...")
        downloader.download_documentation(VEVERKA_ID, output_dir)

        print("\n📂 Kopíruji Unity scripty...")
        downloader.copy_unity_scripts(output_dir)

        print("\n📋 Vytvářím mapping file...")
        downloader.create_mapping_file(output_dir)

        print(f"\n✅ HOTOVO! Vše je připraveno v: {output_dir}")
        print("🔍 Teď můžeš porovnat soubory podle svých potřeb!")

    except Exception as e:
        print(f"❌ Chyba: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    main()