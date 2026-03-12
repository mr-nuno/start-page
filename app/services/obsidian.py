import os
import logging
from datetime import date
from pathlib import Path

log = logging.getLogger(__name__)

VAULT_PATH = Path(os.environ.get("OBSIDIAN_VAULT", "/vault"))
DAILY_DIR = VAULT_PATH / "Daily Notes"


def _today_file():
    return DAILY_DIR / f"{date.today().isoformat()}.md"


def fetch():
    daily_note = ""
    today_file = _today_file()

    if today_file.exists():
        daily_note = today_file.read_text(encoding="utf-8")

    tasks = _find_tasks()
    recent = _recent_notes(5)

    return {
        "daily_note": daily_note,
        "date": date.today().isoformat(),
        "tasks": tasks,
        "recent_notes": recent,
    }


def append_note(text):
    """Append text to today's daily note, creating it if needed."""
    DAILY_DIR.mkdir(parents=True, exist_ok=True)
    today_file = _today_file()

    if not today_file.exists():
        today_file.write_text(f"# {date.today().isoformat()}\n\n", encoding="utf-8")

    with open(today_file, "a", encoding="utf-8") as f:
        f.write(text.rstrip() + "\n")

    return {"ok": True}


def add_task(text):
    """Add a task to today's daily note."""
    return append_note(f"- [ ] {text}")


def toggle_task(filepath, line_num):
    """Toggle a task checkbox in a given file."""
    full_path = VAULT_PATH / filepath
    if not full_path.exists():
        return {"ok": False, "error": "File not found"}

    # Safety: ensure path is within vault
    try:
        full_path.resolve().relative_to(VAULT_PATH.resolve())
    except ValueError:
        return {"ok": False, "error": "Invalid path"}

    lines = full_path.read_text(encoding="utf-8").splitlines(keepends=True)
    idx = line_num - 1
    if 0 <= idx < len(lines):
        line = lines[idx]
        if "- [ ] " in line:
            lines[idx] = line.replace("- [ ] ", "- [x] ", 1)
        elif "- [x] " in line:
            lines[idx] = line.replace("- [x] ", "- [ ] ", 1)
        full_path.write_text("".join(lines), encoding="utf-8")
        return {"ok": True}
    return {"ok": False, "error": "Line not found"}


def _find_tasks(limit=15):
    """Find unchecked tasks across the vault."""
    tasks = []
    if not VAULT_PATH.exists():
        return tasks

    for md in sorted(VAULT_PATH.rglob("*.md"), key=lambda p: p.stat().st_mtime, reverse=True):
        # Skip .obsidian and .trash
        if any(part.startswith(".") for part in md.relative_to(VAULT_PATH).parts):
            continue
        try:
            lines = md.read_text(encoding="utf-8").splitlines()
            rel = str(md.relative_to(VAULT_PATH))
            for i, line in enumerate(lines, 1):
                stripped = line.strip()
                if stripped.startswith("- [ ] "):
                    tasks.append({
                        "text": stripped[6:],
                        "file": rel,
                        "line": i,
                    })
                    if len(tasks) >= limit:
                        return tasks
        except Exception:
            continue
    return tasks


def _recent_notes(limit=5):
    """Return recently modified notes."""
    notes = []
    if not VAULT_PATH.exists():
        return notes

    files = []
    for md in VAULT_PATH.rglob("*.md"):
        if any(part.startswith(".") for part in md.relative_to(VAULT_PATH).parts):
            continue
        files.append(md)

    files.sort(key=lambda p: p.stat().st_mtime, reverse=True)

    for md in files[:limit]:
        rel = str(md.relative_to(VAULT_PATH))
        name = md.stem
        notes.append({"name": name, "path": rel})
    return notes
