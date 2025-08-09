// scripts/export-notion.js
import { Client } from "@notionhq/client";
import { NotionToMarkdown } from "notion-to-md";
import fs from "fs-extra";
import path from "path";
import slugify from "slugify";
import matter from "gray-matter";
import fetch from "node-fetch";

const NOTION_TOKEN = process.env.NOTION_TOKEN;
const ROOT_ID = process.env.NOTION_ROOT_ID; // pageId nebo databaseId

if (!NOTION_TOKEN || !ROOT_ID) {
  console.error("Missing NOTION_TOKEN or NOTION_ROOT_ID.");
  process.exit(1);
}

const docsDir = path.resolve("docs");
await fs.ensureDir(docsDir);

const notion = new Client({ auth: NOTION_TOKEN });
const n2m = new NotionToMarkdown({ notionClient: notion });

// --- Helpery ---
const toSlug = (str) =>
  slugify(str || "untitled", { lower: true, strict: true, trim: true });

async function listChildPagesFromBlocks(blockId) {
  const pages = [];
  let cursor = undefined;
  do {
    const resp = await notion.blocks.children.list({
      block_id: blockId,
      start_cursor: cursor,
      page_size: 100,
    });
    for (const b of resp.results) {
      if (b.type === "child_page") {
        pages.push({ id: b.id, title: b.child_page.title || "Untitled" });
      }
      // Pro jistotu projdi i deeper children (Notion umí child_page v různých hloubkách)
      if (b.has_children) {
        const deeper = await listChildPagesFromBlocks(b.id);
        pages.push(...deeper);
      }
    }
    cursor = resp.has_more ? resp.next_cursor : undefined;
  } while (cursor);
  return pages;
}

async function downloadImagesAndRewrite(md, assetsDir) {
  const imageRegex = /!\[([^\]]*)\]\((https?:\/\/[^\s)]+)\)/g;
  let match;
  const rewrites = [];

  await fs.ensureDir(assetsDir);

  while ((match = imageRegex.exec(md)) !== null) {
    const alt = match[1] || "";
    const url = match[2];
    // stáhni soubor
    try {
      const res = await fetch(url);
      if (!res.ok) continue;
      // odhadni název souboru
      const urlObj = new URL(url);
      const base = path.basename(urlObj.pathname).split("?")[0] || "image";
      const fileName = `${Date.now()}-${base}`;
      const destPath = path.join(assetsDir, fileName);
      const buf = await res.arrayBuffer();
      await fs.writeFile(destPath, Buffer.from(buf));
      const relPath = `./assets/${path.basename(assetsDir)}/${fileName}`;
      rewrites.push({ original: match[0], replacement: `![${alt}](${relPath})` });
    } catch (e) {
      // ignoruj nedostupné URL
    }
  }

  let out = md;
  for (const r of rewrites) {
    out = out.replace(r.original, r.replacement);
  }
  return out;
}

async function exportPage(pageId, parentMeta = null) {
  // Načti základní info o stránce
  const page = await notion.pages.retrieve({ page_id: pageId });
  const titleProp = page.properties?.title || page.properties?.Name;
  let title = "Untitled";
  if (titleProp?.type === "title") {
    title = titleProp.title.map((t) => t.plain_text).join("") || "Untitled";
  } else if (titleProp?.type === "rich_text") {
    title = titleProp.rich_text.map((t) => t.plain_text).join("") || "Untitled";
  } else if (page.icon?.type === "emoji") {
    title = `${page.icon.emoji} ${title}`;
  }

  const slug = toSlug(title);
  const filePath = path.join(docsDir, `${slug}.md`);
  const assetsDir = path.join(docsDir, "assets", slug);

  // Markdown z Notion
  const mdBlocks = await n2m.pageToMarkdown(pageId);
  let mdString = n2m.toMarkdownString(mdBlocks).parent || "";

  // stáhni obrázky a přepiš URL
  mdString = await downloadImagesAndRewrite(mdString, assetsDir);

  // Frontmatter
  const fm = {
    title,
    notionId: pageId,
    parentId: parentMeta?.notionId || null,
    // sem si můžeš přidat tags, lastSynced, apod.
  };
  const fileWithFM = matter.stringify(mdString, fm);

  await fs.writeFile(filePath, fileWithFM, "utf8");
  console.log(`✔ ${title} → docs/${slug}.md`);

  // Najdi child pages a exportuj rekurzivně
  const children = await listChildPagesFromBlocks(pageId);
  for (const ch of children) {
    await exportPage(ch.id, { title, notionId: pageId, slug });
  }
}

async function exportDatabase(databaseId) {
  // Pokud chceš dělat export databáze (každý řádek jako jedna stránka)
  let cursor = undefined;
  do {
    const resp = await notion.databases.query({
      database_id: databaseId,
      start_cursor: cursor,
      page_size: 100,
      // filter/sort si přidej dle potřeby
    });
    for (const row of resp.results) {
      await exportPage(row.id, { notionId: databaseId });
    }
    cursor = resp.has_more ? resp.next_cursor : undefined;
  } while (cursor);
}

(async () => {
  // Vyčisti /docs, ale nech .gitkeep pokud používáš
  await fs.ensureDir(docsDir);

  // —— vyber režim: stránka vs. databáze ——
  try {
    // 1) Zkusit načíst jako stránku
    await exportPage(ROOT_ID, null);
  } catch (e) {
    // 2) Pokud to není stránka, zkus databázi
    if (String(e?.message || "").includes("path failed validation")) {
      await exportDatabase(ROOT_ID);
    } else {
      console.error(e);
      process.exit(1);
    }
  }
})();
