#!/usr/bin/env python3
"""
Notion vs Unity Documentation Comparison
Compares existing Notion documentation with Unity C# scripts
"""

import os
import re
import json
import requests
import time
from pathlib import Path
from typing import Dict, List, Optional, Set
from collections import defaultdict

class NotionUnityCompare:
    def __init__(self, notion_token: str):
        self.notion_token = notion_token
        self.base_url = "https://api.notion.com/v1"
        self.headers = {
            "Authorization": f"Bearer {notion_token}",
            "Content-Type": "application/json",
            "Notion-Version": "2022-06-28"
        }

    def get_notion_pages(self, parent_page_id: str) -> Dict[str, Dict]:
        """Get all pages from Notion documentation (including templates)"""
        print("🔍 Čtu existující Notion dokumentaci (včetně templates)...")

        all_pages = {}

        def get_children(page_id: str, path: str = ""):
            try:
                response = requests.get(
                    f"{self.base_url}/blocks/{page_id}/children?page_size=100",
                    headers=self.headers
                )

                if response.status_code != 200:
                    print(f"❌ Chyba při čtení stránky {page_id}: {response.status_code}")
                    return

                children = response.json().get('results', [])

                for child in children:
                    if child['type'] == 'child_page':
                        title = child['child_page']['title']
                        child_id = child['id']

                        # Skip folder pages (start with 📁)
                        if title.startswith('📁'):
                            folder_name = title.replace('📁 ', '')
                            new_path = f"{path}/{folder_name}" if path else folder_name
                            print(f"📁 Našel složku: {new_path}")
                            get_children(child_id, new_path)
                        # Process script pages (start with 📄)
                        elif title.startswith('📄'):
                            script_name = title.replace('📄 ', '')
                            script_path = f"{path}/{script_name}" if path else script_name

                            all_pages[script_name] = {
                                'title': title,
                                'id': child_id,
                                'path': script_path,
                                'full_path': f"{path}/{script_name}" if path else script_name
                            }
                            print(f"📄 Našel script: {script_path}")
                        else:
                            # Regular page without icon
                            script_path = f"{path}/{title}" if path else title
                            all_pages[title] = {
                                'title': title,
                                'id': child_id,
                                'path': script_path,
                                'full_path': f"{path}/{title}" if path else title
                            }
                            print(f"📄 Našel stránku: {script_path}")

                time.sleep(0.3)  # Rate limiting

            except Exception as e:
                print(f"❌ Chyba při čtení potomků stránky {page_id}: {e}")

        get_children(parent_page_id)
        return all_pages

    def get_unity_scripts(self, root_dir: str = "./Assets") -> Dict[str, Dict]:
        """Get all C# scripts from Unity project"""
        print("\n🔍 Analyzuji Unity scripty...")

        unity_scripts = {}

        for root, dirs, files in os.walk(root_dir):
            # Skip certain directories
            if any(skip_dir in root for skip_dir in ['Editor', 'Tests', 'Test']):
                continue

            for file in files:
                if file.endswith('.cs'):
                    file_path = os.path.join(root, file)
                    rel_path = os.path.relpath(file_path, '.')

                    # Extract class name
                    try:
                        with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                            content = f.read()

                        class_match = re.search(r'public\s+class\s+(\w+)', content)
                        class_name = class_match.group(1) if class_match else Path(file).stem

                        # Get modification time
                        mod_time = os.path.getmtime(file_path)

                        unity_scripts[class_name] = {
                            'class_name': class_name,
                            'file_path': rel_path,
                            'file_name': file,
                            'modified_time': mod_time,
                            'folder': os.path.dirname(rel_path)
                        }

                    except Exception as e:
                        print(f"⚠️  Chyba při čtení {file_path}: {e}")
                        continue

        print(f"📊 Nalezeno {len(unity_scripts)} Unity scriptů")
        return unity_scripts

    def compare_documentation(self, veverka_page_id: str, templates_page_id: str = None) -> Dict:
        """Compare Notion documentation with Unity scripts"""

        # Get data from both sources
        print("🔍 Načítám Veverka dokumentaci...")
        notion_pages = self.get_notion_pages(veverka_page_id)

        if templates_page_id:
            print("🔍 Načítám Templates dokumentaci...")
            template_pages = self.get_notion_pages(templates_page_id)
            # Merge template pages with notion pages, mark them as templates
            for name, info in template_pages.items():
                info['is_template'] = True
                notion_pages[f"Template_{name}"] = info

        unity_scripts = self.get_unity_scripts()

        print(f"\n📊 Notion dokumentace: {len(notion_pages)} stránek")
        print(f"📊 Unity projekt: {len(unity_scripts)} scriptů")

        # Find differences
        notion_names = set(notion_pages.keys())
        unity_names = set(unity_scripts.keys())

        missing_in_notion = unity_names - notion_names
        missing_in_unity = notion_names - unity_names
        documented_scripts = notion_names & unity_names

        # Categorize by folders
        missing_by_folder = defaultdict(list)
        for script_name in missing_in_notion:
            if script_name in unity_scripts:
                folder = unity_scripts[script_name]['folder']
                missing_by_folder[folder].append({
                    'name': script_name,
                    'path': unity_scripts[script_name]['file_path']
                })

        documented_by_folder = defaultdict(list)
        for script_name in documented_scripts:
            if script_name in unity_scripts:
                folder = unity_scripts[script_name]['folder']
                documented_by_folder[folder].append({
                    'name': script_name,
                    'notion_path': notion_pages[script_name]['full_path'],
                    'unity_path': unity_scripts[script_name]['file_path']
                })

        return {
            'notion_pages': notion_pages,
            'unity_scripts': unity_scripts,
            'missing_in_notion': missing_in_notion,
            'missing_in_unity': missing_in_unity,
            'documented_scripts': documented_scripts,
            'missing_by_folder': dict(missing_by_folder),
            'documented_by_folder': dict(documented_by_folder),
            'stats': {
                'total_notion': len(notion_pages),
                'total_unity': len(unity_scripts),
                'documented': len(documented_scripts),
                'missing_docs': len(missing_in_notion),
                'orphaned_docs': len(missing_in_unity)
            }
        }

    def generate_report(self, comparison: Dict) -> str:
        """Generate a detailed comparison report"""

        report = []
        stats = comparison['stats']

        report.append("=" * 60)
        report.append("🔍 NOTION vs UNITY DOKUMENTACE - POROVNÁNÍ")
        report.append("=" * 60)

        # Summary stats
        report.append(f"\n📊 STATISTIKY:")
        report.append(f"   • Unity scripty celkem: {stats['total_unity']}")
        report.append(f"   • Notion stránky celkem: {stats['total_notion']}")
        report.append(f"   • Zdokumentované scripty: {stats['documented']}")
        report.append(f"   • Chybí dokumentace: {stats['missing_docs']}")
        report.append(f"   • Dokumentace navíc: {stats['orphaned_docs']}")

        coverage = (stats['documented'] / stats['total_unity'] * 100) if stats['total_unity'] > 0 else 0
        report.append(f"   • Pokrytí dokumentací: {coverage:.1f}%")

        # Missing documentation by folder
        if comparison['missing_by_folder']:
            report.append(f"\n❌ CHYBÍ DOKUMENTACE ({stats['missing_docs']} scriptů):")
            for folder, scripts in sorted(comparison['missing_by_folder'].items()):
                report.append(f"\n📁 {folder or 'Root'}:")
                for script in sorted(scripts, key=lambda x: x['name']):
                    report.append(f"   • {script['name']} ({script['path']})")

        # Documented scripts by folder
        if comparison['documented_by_folder']:
            report.append(f"\n✅ ZDOKUMENTOVANÉ SCRIPTY ({stats['documented']} scriptů):")
            for folder, scripts in sorted(comparison['documented_by_folder'].items()):
                if scripts:  # Only show folders with scripts
                    report.append(f"\n📁 {folder or 'Root'}:")
                    for script in sorted(scripts, key=lambda x: x['name']):
                        report.append(f"   • {script['name']}")

        # Orphaned documentation
        if comparison['missing_in_unity']:
            report.append(f"\n⚠️  DOKUMENTACE BEZ UNITY SCRIPTU ({stats['orphaned_docs']}):")
            for script_name in sorted(comparison['missing_in_unity']):
                notion_info = comparison['notion_pages'][script_name]
                report.append(f"   • {script_name} (Notion: {notion_info['full_path']})")

        report.append(f"\n{'=' * 60}")

        return "\n".join(report)

def main():
    # Configuration
    NOTION_TOKEN = "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"

    # Get parent page IDs
    PARENT_PAGE_ID = input("Zadej ID Veverka stránky s dokumentací: ").strip()
    if not PARENT_PAGE_ID:
        print("❌ Musíš zadat ID Veverka stránky!")
        return

    templates_page_id = input("Zadej ID Templates stránky (nebo Enter pro přeskočení): ").strip()
    if not templates_page_id:
        templates_page_id = None
        print("⚠️ Templates stránka bude přeskočena")

    # Initialize comparison tool
    comparer = NotionUnityCompare(NOTION_TOKEN)

    # Run comparison
    try:
        comparison = comparer.compare_documentation(PARENT_PAGE_ID, templates_page_id)

        # Generate and display report
        report = comparer.generate_report(comparison)
        print(report)

        # Save report to file
        with open("notion_unity_comparison.txt", "w", encoding="utf-8") as f:
            f.write(report)

        print(f"\n💾 Report uložen do: notion_unity_comparison.txt")

    except Exception as e:
        print(f"❌ Chyba během porovnání: {e}")

if __name__ == "__main__":
    main()