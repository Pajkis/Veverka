#!/usr/bin/env python3
"""
Complete Notion Documentation Analysis
Autonomously analyzes documentation vs templates vs Unity scripts
"""

import os
import re
import json
import requests
import time
from pathlib import Path
from typing import Dict, List, Optional, Set
from collections import defaultdict

class NotionCompleteAnalysis:
    def __init__(self, notion_token: str):
        self.notion_token = notion_token
        self.base_url = "https://api.notion.com/v1"
        self.headers = {
            "Authorization": f"Bearer {notion_token}",
            "Content-Type": "application/json",
            "Notion-Version": "2022-06-28"
        }
        self.templates = {}
        self.documentation_pages = {}
        self.unity_scripts = {}

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
            content_text = []
            sections = {}
            current_section = None

            for block in blocks:
                block_text = self.extract_text_from_block(block)
                if block_text:
                    content_text.append(block_text)

                    # Identify sections (headings)
                    if block['type'] in ['heading_1', 'heading_2', 'heading_3']:
                        current_section = block_text
                        sections[current_section] = []
                    elif current_section and block_text != current_section:
                        sections[current_section].append(block_text)

            return {
                'title': self.extract_title_from_page(page_data),
                'content': '\n'.join(content_text),
                'sections': sections,
                'blocks': blocks
            }

        except Exception as e:
            print(f"❌ Chyba při čtení stránky {page_id}: {e}")
            return {}

    def extract_text_from_block(self, block: Dict) -> str:
        """Extract text from a Notion block"""
        try:
            block_type = block.get('type', '')

            if block_type in ['paragraph', 'heading_1', 'heading_2', 'heading_3']:
                rich_text = block.get(block_type, {}).get('rich_text', [])
                return ''.join([text.get('plain_text', '') for text in rich_text])

            elif block_type == 'bulleted_list_item':
                rich_text = block.get('bulleted_list_item', {}).get('rich_text', [])
                return '• ' + ''.join([text.get('plain_text', '') for text in rich_text])

            elif block_type == 'numbered_list_item':
                rich_text = block.get('numbered_list_item', {}).get('rich_text', [])
                return '1. ' + ''.join([text.get('plain_text', '') for text in rich_text])

            elif block_type == 'code':
                rich_text = block.get('code', {}).get('rich_text', [])
                return '```\n' + ''.join([text.get('plain_text', '') for text in rich_text]) + '\n```'

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

    def load_all_notion_data(self, veverka_id: str, templates_id: str):
        """Load all data from Notion workspaces"""
        print("🔍 Načítám Templates...")
        self.load_templates(templates_id)

        print("🔍 Načítám dokumentaci...")
        self.load_documentation(veverka_id)

        print("🔍 Načítám Unity scripty...")
        self.load_unity_scripts()

    def load_templates(self, templates_id: str):
        """Load and analyze all templates"""
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

                        # Templates start with "_Template"
                        if title.startswith('_Template'):
                            template_type = self.identify_template_type(title)
                            content = self.get_page_content(child_id)

                            self.templates[template_type] = {
                                'title': title,
                                'id': child_id,
                                'content': content,
                                'structure': self.analyze_template_structure(content)
                            }
                            print(f"📋 Template načten: {template_type}")

                        process_children(child_id, f"{path}/{title}")

                time.sleep(0.3)

            except Exception as e:
                print(f"❌ Chyba při načítání templates: {e}")

        process_children(templates_id)

    def identify_template_type(self, title: str) -> str:
        """Identify what type of template this is"""
        title_lower = title.lower()

        if 'class' in title_lower:
            return 'class'
        elif 'component' in title_lower:
            return 'component'
        elif 'event' in title_lower:
            return 'event'
        elif 'payload' in title_lower:
            return 'payload'
        elif 'enum' in title_lower:
            return 'enum'
        elif 'scene' in title_lower:
            return 'scene'
        elif 'interface' in title_lower:
            return 'interface'
        elif 'feature' in title_lower:
            return 'feature'
        else:
            return 'generic'

    def analyze_template_structure(self, content: Dict) -> Dict:
        """Analyze the structure of a template"""
        if not content or 'sections' not in content:
            return {}

        structure = {
            'required_sections': [],
            'optional_sections': [],
            'section_patterns': {}
        }

        for section_name, section_content in content['sections'].items():
            structure['required_sections'].append(section_name)

            # Analyze what should be in each section
            section_text = ' '.join(section_content).lower()
            patterns = []

            if 'cesta' in section_text or 'path' in section_text:
                patterns.append('file_path')
            if 'popis' in section_text or 'description' in section_text:
                patterns.append('description')
            if 'metod' in section_text or 'method' in section_text:
                patterns.append('methods')
            if 'proměnn' in section_text or 'variable' in section_text or 'field' in section_text:
                patterns.append('fields')
            if 'událost' in section_text or 'event' in section_text:
                patterns.append('events')
            if 'dědí' in section_text or 'inherit' in section_text:
                patterns.append('inheritance')

            structure['section_patterns'][section_name] = patterns

        return structure

    def load_documentation(self, veverka_id: str):
        """Load all documentation pages"""
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

                        # Skip folder pages (📁) and templates
                        if title.startswith('📁'):
                            folder_name = title.replace('📁 ', '')
                            new_path = f"{path}/{folder_name}" if path else folder_name
                            process_children(child_id, new_path)
                        elif title.startswith('📄'):
                            # Documentation page
                            script_name = title.replace('📄 ', '')
                            content = self.get_page_content(child_id)

                            self.documentation_pages[script_name] = {
                                'title': title,
                                'id': child_id,
                                'path': path,
                                'content': content,
                                'script_name': script_name
                            }
                            print(f"📄 Dokumentace načtena: {script_name}")
                        elif not title.startswith('_') and title not in ['Changelogs', 'Introduction', 'Software']:
                            # Regular documentation page
                            content = self.get_page_content(child_id)

                            self.documentation_pages[title] = {
                                'title': title,
                                'id': child_id,
                                'path': path,
                                'content': content,
                                'script_name': title
                            }
                            print(f"📄 Dokumentace načtena: {title}")

                time.sleep(0.3)

            except Exception as e:
                print(f"❌ Chyba při načítání dokumentace: {e}")

        process_children(veverka_id)

    def load_unity_scripts(self):
        """Load and analyze Unity scripts"""
        for root, dirs, files in os.walk("./Assets"):
            # Skip certain directories
            if any(skip_dir in root for skip_dir in ['Editor', 'Tests', 'Test', 'TextMesh Pro']):
                continue

            for file in files:
                if file.endswith('.cs'):
                    file_path = os.path.join(root, file)
                    rel_path = os.path.relpath(file_path, '.')

                    try:
                        with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                            content = f.read()

                        analysis = self.analyze_unity_script(content, rel_path)
                        if analysis:
                            self.unity_scripts[analysis['class_name']] = analysis

                    except Exception as e:
                        print(f"⚠️ Chyba při čtení {file_path}: {e}")
                        continue

    def analyze_unity_script(self, content: str, file_path: str) -> Dict:
        """Analyze a Unity script and extract all information"""
        analysis = {
            'file_path': file_path,
            'content': content
        }

        # Extract class info
        class_match = re.search(r'public\s+class\s+(\w+)(?:\s*:\s*(.+?))?(?:\s*{|\s*$)', content, re.MULTILINE)
        analysis['class_name'] = class_match.group(1) if class_match else Path(file_path).stem
        analysis['base_class'] = class_match.group(2).strip() if class_match and class_match.group(2) else None

        # Extract XML summary
        summary_match = re.search(r'/// <summary>\s*\n(.+?)\s*\n.*?/// </summary>', content, re.DOTALL)
        analysis['xml_summary'] = summary_match.group(1).replace('///', '').strip() if summary_match else ""

        # Extract methods with details
        methods = []
        method_pattern = r'(?:/// <summary>\s*\n(.+?)\n.*?/// </summary>\s*\n.*?)?((?:public|protected|private)\s+(?:override\s+)?(?:virtual\s+)?(\w+(?:<.+?>)?)\s+(\w+)\s*\([^)]*\))'
        for match in re.finditer(method_pattern, content, re.DOTALL | re.MULTILINE):
            method_summary = match.group(1).replace('///', '').strip() if match.group(1) else ""
            full_signature = match.group(2)
            return_type = match.group(3)
            method_name = match.group(4)

            methods.append({
                'name': method_name,
                'return_type': return_type,
                'signature': full_signature,
                'summary': method_summary,
                'has_documentation': bool(method_summary)
            })

        analysis['methods'] = methods

        # Extract fields
        fields = []
        field_patterns = [
            r'\[SerializeField\].*?(\w+)\s+(\w+);',  # SerializeField
            r'(?:public|protected|private)\s+(\w+)\s+(\w+)\s*[;=]'  # Regular fields
        ]

        for pattern in field_patterns:
            for match in re.finditer(pattern, content):
                field_type = match.group(1)
                field_name = match.group(2)
                fields.append({
                    'name': field_name,
                    'type': field_type,
                    'is_serialized': 'SerializeField' in match.group(0)
                })

        analysis['fields'] = fields

        # Extract events
        events = []
        event_pattern = r'(\w*Event\w*)\s+(\w+)'
        for match in re.finditer(event_pattern, content):
            events.append({
                'type': match.group(1),
                'name': match.group(2)
            })

        analysis['events'] = events

        # Determine script type
        analysis['script_type'] = self.determine_script_type(content, analysis['class_name'])

        return analysis

    def determine_script_type(self, content: str, class_name: str) -> str:
        """Determine what type of Unity script this is"""
        class_name_lower = class_name.lower()

        if 'payload' in class_name_lower:
            return 'payload'
        elif 'event' in class_name_lower:
            return 'event'
        elif 'enum' in class_name_lower or 'public enum' in content:
            return 'enum'
        elif 'interface' in content or class_name.startswith('I'):
            return 'interface'
        elif 'MonoBehaviour' in content:
            return 'component'
        elif 'ScriptableObject' in content:
            return 'scriptable_object'
        else:
            return 'class'

    def compare_documentation_vs_script(self, doc_name: str) -> Dict:
        """Compare documentation with actual Unity script"""
        if doc_name not in self.documentation_pages or doc_name not in self.unity_scripts:
            return {}

        doc = self.documentation_pages[doc_name]
        script = self.unity_scripts[doc_name]

        comparison = {
            'doc_name': doc_name,
            'differences': [],
            'missing_in_doc': [],
            'extra_in_doc': [],
            'matches': []
        }

        # Compare methods
        doc_content = doc['content']['content'].lower()
        doc_methods = re.findall(r'(\w+)\s*\(', doc_content)
        script_methods = [m['name'] for m in script['methods']]

        for method in script_methods:
            if method.lower() not in doc_content:
                comparison['missing_in_doc'].append(f"Method: {method}")

        # Compare fields
        script_fields = [f['name'] for f in script['fields']]
        for field in script_fields:
            if field.lower() not in doc_content:
                comparison['missing_in_doc'].append(f"Field: {field}")

        # Check if inheritance is documented
        if script['base_class'] and 'dědí' not in doc_content and 'inherit' not in doc_content:
            comparison['missing_in_doc'].append(f"Inheritance: {script['base_class']}")

        return comparison

    def compare_documentation_vs_template(self, doc_name: str) -> Dict:
        """Compare documentation with appropriate template"""
        if doc_name not in self.documentation_pages:
            return {}

        doc = self.documentation_pages[doc_name]
        script_type = self.unity_scripts.get(doc_name, {}).get('script_type', 'class')

        # Find appropriate template
        template = self.templates.get(script_type, self.templates.get('class', {}))
        if not template:
            return {}

        comparison = {
            'doc_name': doc_name,
            'template_type': script_type,
            'missing_sections': [],
            'extra_sections': [],
            'section_quality': {}
        }

        doc_sections = set(doc['content'].get('sections', {}).keys())
        required_sections = set(template.get('structure', {}).get('required_sections', []))

        # Check missing sections
        for section in required_sections:
            if section not in doc_sections:
                comparison['missing_sections'].append(section)

        # Check extra sections
        for section in doc_sections:
            if section not in required_sections:
                comparison['extra_sections'].append(section)

        # Analyze section quality
        for section, patterns in template.get('structure', {}).get('section_patterns', {}).items():
            if section in doc_sections:
                section_content = ' '.join(doc['content']['sections'][section]).lower()
                quality = {'expected_patterns': patterns, 'found_patterns': [], 'missing_patterns': []}

                for pattern in patterns:
                    pattern_found = False
                    if pattern == 'file_path' and ('cesta' in section_content or '.cs' in section_content):
                        pattern_found = True
                    elif pattern == 'description' and len(section_content) > 20:
                        pattern_found = True
                    elif pattern == 'methods' and any(method in section_content for method in ['metod', 'function', '()']):
                        pattern_found = True
                    elif pattern == 'fields' and any(field in section_content for field in ['proměnn', 'field', 'variable']):
                        pattern_found = True

                    if pattern_found:
                        quality['found_patterns'].append(pattern)
                    else:
                        quality['missing_patterns'].append(pattern)

                comparison['section_quality'][section] = quality

        return comparison

    def generate_complete_report(self) -> str:
        """Generate comprehensive analysis report"""
        report = []

        report.append("=" * 80)
        report.append("🔍 KOMPLETNÍ ANALÝZA NOTION DOKUMENTACE")
        report.append("=" * 80)

        # Overall statistics
        total_docs = len(self.documentation_pages)
        total_scripts = len(self.unity_scripts)
        total_templates = len(self.templates)

        report.append(f"\n📊 CELKOVÉ STATISTIKY:")
        report.append(f"   • Dokumentace stránek: {total_docs}")
        report.append(f"   • Unity scriptů: {total_scripts}")
        report.append(f"   • Templates: {total_templates}")

        # Analyze each documentation
        report.append(f"\n📋 DETAILNÍ ANALÝZA DOKUMENTACE:")
        report.append("=" * 60)

        for doc_name in self.documentation_pages.keys():
            report.append(f"\n📄 {doc_name}")
            report.append("-" * 40)

            # Compare with script
            script_comparison = self.compare_documentation_vs_script(doc_name)
            if script_comparison:
                if script_comparison['missing_in_doc']:
                    report.append(f"❌ Chybí v dokumentaci:")
                    for item in script_comparison['missing_in_doc'][:5]:  # Limit to 5 items
                        report.append(f"   • {item}")

            # Compare with template
            template_comparison = self.compare_documentation_vs_template(doc_name)
            if template_comparison:
                if template_comparison['missing_sections']:
                    report.append(f"📋 Chybí sekce z template:")
                    for section in template_comparison['missing_sections']:
                        report.append(f"   • {section}")

                if template_comparison['section_quality']:
                    report.append(f"🔍 Kvalita sekcí:")
                    for section, quality in template_comparison['section_quality'].items():
                        if quality['missing_patterns']:
                            report.append(f"   • {section}: chybí {', '.join(quality['missing_patterns'])}")

            report.append("")

        # Template analysis
        report.append(f"\n📋 ANALÝZA TEMPLATES:")
        report.append("=" * 60)
        for template_type, template in self.templates.items():
            required_sections = template.get('structure', {}).get('required_sections', [])
            report.append(f"📋 {template_type.upper()} Template:")
            report.append(f"   • Požadované sekce: {', '.join(required_sections)}")

        return "\n".join(report)

def main():
    # Configuration
    NOTION_TOKEN = "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"
    VEVERKA_ID = "20547dcf01688077a81cf9a7d7d9fed5"
    TEMPLATES_ID = "21f47dcf016880809142ec53df8b2c18"

    print("🚀 Spouštím kompletní analýzu...")

    # Initialize analyzer
    analyzer = NotionCompleteAnalysis(NOTION_TOKEN)

    try:
        # Load all data
        analyzer.load_all_notion_data(VEVERKA_ID, TEMPLATES_ID)

        # Generate comprehensive report
        report = analyzer.generate_complete_report()
        print(report)

        # Save report
        with open("complete_notion_analysis.txt", "w", encoding="utf-8") as f:
            f.write(report)

        print(f"\n💾 Kompletní analýza uložena do: complete_notion_analysis.txt")

    except Exception as e:
        print(f"❌ Chyba během analýzy: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    main()