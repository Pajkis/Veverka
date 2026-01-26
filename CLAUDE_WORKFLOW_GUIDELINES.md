# Development Template

## Workflow:

**Planning Phase:**
- Na začátku vždy společně vytvoříme plán v češtině nebo angličtině pro novou feature
- Označit oblasti, které se budou změny týkat a řádně je prostudovat
- Identifikovat ovlivněné soubory/systémy před začátkem implementace
- Rozbít větší features na menší, testovatelné kroky
- Ověřit existující patterns v codebase (jak podobné věci už jsou řešené)

**Implementation Phase:**
- Preferovat úpravu existujících souborů před vytvářením nových
- Commits - vytvářet pouze na explicitní požádání
- Testing - otestovat změny v Unity editoru před dokončením úkolu

**Review Phase:**
- Projít TODO list a ověřit, že všechny kroky jsou hotové
- Highlight případných trade-offs nebo věcí k budoucímu zlepšení

**Communication:**
- Pokud najdu něco podivného/suboptimálního v existujícím kódu, upozornit
- Ptát se při nejasnostech místo předpokladů
- Používat markdown odkazy na soubory pro snadnou navigaci

## Code Standards:
- Komentáře v kódu vždy v AJ
- Pro logy používat DebugLogger
- Separovat logiku UI a hry/herních objektů
- **Žádná magická čísla** - vždy použít konfigurační soubor (pokud si nejsem jistý, konzultovat)
- Upozornit na nalezená magická čísla v existujícím kódu

## Best Practices:
- Dodržovat C# naming conventions
- Single Responsibility Principle - krátké, zaměřené metody
- Preferovat composition před inheritance
- Používat ScriptableObjects pro konfiguraci
- Unity Events/custom event system místo přímých referencí
- Cachovat GetComponent calls, nepoužívat Find* v Update
- Validovat public/serialized fields (RequireComponent, null checks)
- Assertions pro kritické předpoklady
- Editor debug tools (Gizmos) kde to dává smysl
