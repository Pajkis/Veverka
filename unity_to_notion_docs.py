#!/usr/bin/env python3
"""
Unity to Notion Documentation Generator
Automatically generates documentation for Unity C# scripts and uploads to Notion
"""

import os
import re
import json
import requests
import time
from pathlib import Path
from typing import Dict, List, Optional, Tuple

class UnityToNotionDocs:
    def __init__(self, notion_token: str, notion_database_id: Optional[str] = None):
        self.notion_token = notion_token
        self.notion_database_id = notion_database_id
        self.base_url = "https://api.notion.com/v1"
        self.headers = {
            "Authorization": f"Bearer {notion_token}",
            "Content-Type": "application/json",
            "Notion-Version": "2022-06-28"
        }
        self.created_pages = {}  # Cache for created folder pages

    def analyze_cs_file(self, file_path: str) -> Dict:
        """Analyze a C# Unity script and extract documentation info"""
        try:
            with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                content = f.read()
        except Exception as e:
            print(f"Error reading {file_path}: {e}")
            return {}

        # Extract class info
        class_match = re.search(r'public\s+class\s+(\w+)(?:\s*:\s*(.+?))?(?:\s*{|\s*$)', content, re.MULTILINE)
        class_name = class_match.group(1) if class_match else Path(file_path).stem
        base_class = class_match.group(2).strip() if class_match and class_match.group(2) else None

        # Extract summary from XML comments
        summary_match = re.search(r'/// <summary>\s*\n(.+?)\s*\n.*?/// </summary>', content, re.DOTALL)
        summary = summary_match.group(1).replace('///', '').strip() if summary_match else ""

        # Extract public methods
        methods = []
        method_pattern = r'(?:/// <summary>\s*\n(.+?)\n.*?/// </summary>\s*\n.*?)?(?:public|protected)\s+(?:override\s+)?(?:virtual\s+)?(\w+(?:<.+?>)?)\s+(\w+)\s*\([^)]*\)'
        for match in re.finditer(method_pattern, content, re.DOTALL | re.MULTILINE):
            method_summary = match.group(1).replace('///', '').strip() if match.group(1) else ""
            return_type = match.group(2)
            method_name = match.group(3)
            methods.append({
                'name': method_name,
                'return_type': return_type,
                'summary': method_summary
            })

        # Extract serialized fields
        fields = []
        field_pattern = r'\[SerializeField\].*?(\w+)\s+(\w+);'
        for match in re.finditer(field_pattern, content):
            field_type = match.group(1)
            field_name = match.group(2)
            fields.append({
                'name': field_name,
                'type': field_type
            })

        # Detect Unity components/features
        components = []
        if 'MonoBehaviour' in content:
            components.append('MonoBehaviour')
        if 'ScriptableObject' in content:
            components.append('ScriptableObject')
        if re.search(r'Start\s*\(', content):
            components.append('Uses Start()')
        if re.search(r'Update\s*\(', content):
            components.append('Uses Update()')
        if re.search(r'OnEnable\s*\(', content):
            components.append('Uses OnEnable()')
        if re.search(r'OnDisable\s*\(', content):
            components.append('Uses OnDisable()')

        # Extract relative path for better organization
        rel_path = os.path.relpath(file_path, '.')
        path_parts = rel_path.split(os.sep)
        category = path_parts[1] if len(path_parts) > 1 else "Scripts"
        folder_path = os.sep.join(path_parts[:-1]) if len(path_parts) > 1 else ""

        return {
            'file_path': rel_path,
            'folder_path': folder_path,
            'class_name': class_name,
            'base_class': base_class,
            'summary': summary,
            'category': category,
            'methods': methods,
            'fields': fields,
            'components': components,
            'content': content
        }

    def find_cs_files(self, root_dir: str = "./Assets") -> List[str]:
        """Find all C# files in the Unity project"""
        cs_files = []
        for root, dirs, files in os.walk(root_dir):
            # Skip certain directories
            if any(skip_dir in root for skip_dir in ['Editor', 'Tests', 'Test']):
                continue

            for file in files:
                if file.endswith('.cs'):
                    cs_files.append(os.path.join(root, file))

        return sorted(cs_files)

    def create_folder_page(self, folder_name: str, parent_id: str, folder_path: str) -> Optional[str]:
        """Create a folder page in Notion and return its ID"""

        # Check if we already created this folder
        if folder_path in self.created_pages:
            return self.created_pages[folder_path]

        page_data = {
            "parent": {"page_id": parent_id},
            "properties": {
                "title": {
                    "title": [
                        {
                            "type": "text",
                            "text": {"content": f"📁 {folder_name}"}
                        }
                    ]
                }
            },
            "children": [
                {
                    "object": "block",
                    "type": "paragraph",
                    "paragraph": {
                        "rich_text": [
                            {"type": "text", "text": {"content": f"Složka: {folder_path}"}}
                        ]
                    }
                }
            ]
        }

        try:
            response = requests.post(
                f"{self.base_url}/pages",
                headers=self.headers,
                json=page_data
            )

            if response.status_code == 200:
                page_id = response.json()['id']
                self.created_pages[folder_path] = page_id
                print(f"📁 Vytvořena složka: {folder_name}")
                return page_id
            else:
                print(f"❌ Chyba při vytváření složky {folder_name}: {response.status_code}")
                return None

        except Exception as e:
            print(f"❌ Chyba při vytváření složky {folder_name}: {e}")
            return None

    def get_or_create_folder_hierarchy(self, folder_path: str, root_parent_id: str) -> str:
        """Create folder hierarchy and return the final parent ID"""
        if not folder_path or folder_path == ".":
            return root_parent_id

        path_parts = folder_path.split(os.sep)
        current_parent = root_parent_id
        current_path = ""

        for i, folder_name in enumerate(path_parts):
            if not folder_name:
                continue

            current_path = os.sep.join(path_parts[:i+1])

            # Skip 'Assets' folder as it's the root
            if folder_name == "Assets":
                continue

            folder_id = self.create_folder_page(folder_name, current_parent, current_path)
            if folder_id:
                current_parent = folder_id

            # Rate limiting
            time.sleep(0.2)

        return current_parent

    def create_notion_page(self, script_info: Dict, root_parent_id: str) -> bool:
        """Create a new Notion page for a Unity script"""

        # Get or create the folder hierarchy for this script
        correct_parent_id = self.get_or_create_folder_hierarchy(script_info['folder_path'], root_parent_id)

        # Create page content
        children = [
            {
                "object": "block",
                "type": "heading_2",
                "heading_2": {
                    "rich_text": [{"type": "text", "text": {"content": "Přehled"}}]
                }
            },
            {
                "object": "block",
                "type": "paragraph",
                "paragraph": {
                    "rich_text": [
                        {"type": "text", "text": {"content": f"📁 Cesta: "}},
                        {"type": "text", "text": {"content": script_info['file_path']}, "annotations": {"code": True}}
                    ]
                }
            }
        ]

        if script_info['base_class']:
            children.append({
                "object": "block",
                "type": "paragraph",
                "paragraph": {
                    "rich_text": [
                        {"type": "text", "text": {"content": f"🔗 Dědí od: "}},
                        {"type": "text", "text": {"content": script_info['base_class']}, "annotations": {"code": True}}
                    ]
                }
            })

        if script_info['summary']:
            children.extend([
                {
                    "object": "block",
                    "type": "heading_3",
                    "heading_3": {
                        "rich_text": [{"type": "text", "text": {"content": "Popis"}}]
                    }
                },
                {
                    "object": "block",
                    "type": "paragraph",
                    "paragraph": {
                        "rich_text": [{"type": "text", "text": {"content": script_info['summary']}}]
                    }
                }
            ])

        # Add Unity components info
        if script_info['components']:
            children.extend([
                {
                    "object": "block",
                    "type": "heading_3",
                    "heading_3": {
                        "rich_text": [{"type": "text", "text": {"content": "Unity komponenty"}}]
                    }
                },
                {
                    "object": "block",
                    "type": "bulleted_list_item",
                    "bulleted_list_item": {
                        "rich_text": [{"type": "text", "text": {"content": ", ".join(script_info['components'])}}]
                    }
                }
            ])

        # Add serialized fields
        if script_info['fields']:
            children.append({
                "object": "block",
                "type": "heading_3",
                "heading_3": {
                    "rich_text": [{"type": "text", "text": {"content": "Serializované proměnné"}}]
                }
            })

            for field in script_info['fields']:
                children.append({
                    "object": "block",
                    "type": "bulleted_list_item",
                    "bulleted_list_item": {
                        "rich_text": [
                            {"type": "text", "text": {"content": f"{field['name']}"}, "annotations": {"bold": True}},
                            {"type": "text", "text": {"content": f" ({field['type']})"}}
                        ]
                    }
                })

        # Add methods
        if script_info['methods']:
            children.append({
                "object": "block",
                "type": "heading_3",
                "heading_3": {
                    "rich_text": [{"type": "text", "text": {"content": "Metody"}}]
                }
            })

            for method in script_info['methods']:
                method_text = [
                    {"type": "text", "text": {"content": f"{method['name']}()"}, "annotations": {"code": True}},
                    {"type": "text", "text": {"content": f" → {method['return_type']}"}}
                ]

                if method['summary']:
                    method_text.extend([
                        {"type": "text", "text": {"content": f"\n{method['summary']}"}}
                    ])

                children.append({
                    "object": "block",
                    "type": "bulleted_list_item",
                    "bulleted_list_item": {
                        "rich_text": method_text
                    }
                })

        # Create the page
        page_data = {
            "parent": {"page_id": correct_parent_id},
            "properties": {
                "title": {
                    "title": [
                        {
                            "type": "text",
                            "text": {"content": f"📄 {script_info['class_name']}"}
                        }
                    ]
                }
            },
            "children": children
        }

        try:
            response = requests.post(
                f"{self.base_url}/pages",
                headers=self.headers,
                json=page_data
            )

            if response.status_code == 200:
                print(f"✅ Vytvořena dokumentace pro: {script_info['class_name']}")
                return True
            else:
                print(f"❌ Chyba při vytváření {script_info['class_name']}: {response.status_code}")
                print(response.text)
                return False

        except Exception as e:
            print(f"❌ Chyba při vytváření {script_info['class_name']}: {e}")
            return False

    def generate_documentation(self, parent_page_id: str, max_files: int = 50):
        """Generate documentation for Unity scripts"""
        print("🔍 Hledám C# scripty...")
        cs_files = self.find_cs_files()
        print(f"📊 Nalezeno {len(cs_files)} C# souborů")

        if max_files and len(cs_files) > max_files:
            print(f"⚠️  Omezuji na prvních {max_files} souborů pro test")
            cs_files = cs_files[:max_files]

        successful = 0
        failed = 0

        for i, file_path in enumerate(cs_files, 1):
            print(f"\n📄 [{i}/{len(cs_files)}] Zpracovávám: {file_path}")

            # Analyze the script
            script_info = self.analyze_cs_file(file_path)
            if not script_info:
                print(f"⚠️  Přeskakuji {file_path} - nepodařilo se analyzovat")
                failed += 1
                continue

            # Create Notion page
            if self.create_notion_page(script_info, parent_page_id):
                successful += 1
            else:
                failed += 1

            # Rate limiting - Notion allows 3 requests per second
            time.sleep(0.4)

        print(f"\n🎉 Dokončeno! ✅ {successful} úspěšných, ❌ {failed} neúspěšných")

def main():
    # Configuration
    NOTION_TOKEN = "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"

    # You need to provide the parent page ID where documentation will be created
    # This should be a page you have access to in your Notion workspace
    PARENT_PAGE_ID = input("Zadej ID stránky v Notion, kam chceš vytvořit dokumentaci: ").strip()

    if not PARENT_PAGE_ID:
        print("❌ Musíš zadat ID stránky!")
        return

    # Initialize the documentation generator
    doc_generator = UnityToNotionDocs(NOTION_TOKEN)

    # Generate documentation (limited to 25 files for testing the folder structure)
    doc_generator.generate_documentation(PARENT_PAGE_ID, max_files=25)

if __name__ == "__main__":
    main()