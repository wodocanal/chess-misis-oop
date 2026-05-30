#!/usr/bin/env python3
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).parent if "__file__" in globals() else Path.cwd()
EXCLUDE_DIRS = {".git", "bin", "obj", "Debug", "Release", "node_modules", ".idea", ".vscode"}

HEADER_CS = (
    "// SPDX-License-Identifier: GPL-3.0-or-later\n"
    "// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>\n"
    "// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)\n\n"
)

HEADER_AXAML = (
    "<!-- SPDX-License-Identifier: GPL-3.0-or-later -->\n"
    "<!-- SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com> -->\n"
    "<!-- SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal) -->\n\n"
)

MARKER = "SPDX-License-Identifier: GPL-3.0-or-later"

def has_header(content: str, marker: str) -> bool:
    return marker in content

def needs_header(file_path: Path) -> bool:
    if any(excl in file_path.parts for excl in EXCLUDE_DIRS):
        return False
    return file_path.suffix in {".cs", ".axaml"}

def add_header(file_path: Path):
    with open(file_path, "r", encoding="utf-8") as f:
        original = f.read()

    if has_header(original, MARKER):
        print(f"SKIP (already has header): {file_path}")
        return

    # Determine header
    if file_path.suffix == ".cs":
        header = HEADER_CS
    else:  # .axaml
        header = HEADER_AXAML
        # If file starts with XML declaration, insert after it
        if original.lstrip().startswith("<?xml"):
            lines = original.splitlines(keepends=True)
            for i, line in enumerate(lines):
                if line.lstrip().startswith("<?xml"):
                    lines.insert(i + 1, header)
                    break
            new_content = "".join(lines)
        else:
            new_content = header + original
    if file_path.suffix == ".cs":
        new_content = header + original

    with open(file_path, "w", encoding="utf-8") as f:
        f.write(new_content)
    print(f"UPDATED: {file_path}")

def main():
    files = list(REPO_ROOT.rglob("*"))
    changed = 0
    for fp in files:
        if fp.is_file() and needs_header(fp):
            add_header(fp)
            changed += 1
    print(f"\nDone. Updated {changed} file(s).")

if __name__ == "__main__":
    main()