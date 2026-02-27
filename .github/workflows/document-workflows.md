---
description: |
  This workflow automatically creates or updates documentation for GitHub Actions
  workflows. Whenever a YML file is changed in the .github/workflows folder, it
  generates or updates a corresponding markdown file in the docs/ folder that
  describes all steps inside the workflow.

on:
  push:
    branches: [main]
    paths:
      - .github/workflows/*.yml

permissions:
  contents: read

tools:
  edit:
  bash: true

safe-outputs:
  create-pull-request:
    title-prefix: "[docs] "
    labels: [documentation, automated]
    draft: false
    if-no-changes: warn

engine: copilot
---

# Document Workflows

When a YML workflow file is changed in `.github/workflows/`, create or update its documentation in the `docs/` folder.

## Task

For each YML file modified in this push event, generate or update the corresponding markdown documentation file in `docs/`.

## Steps

1. Identify the YML files that were changed in this push event by running:
   ```bash
   git diff --name-only HEAD~1 HEAD -- '.github/workflows/*.yml'
   ```
   If that command fails or returns nothing, check which files were changed using git log.

2. For each changed YML file (e.g., `.github/workflows/my-workflow.yml`):

   a. Read the full content of the YML file.

   b. Analyze the workflow structure, including:
      - Workflow name
      - Triggers (`on:` section)
      - Permissions
      - Environment variables
      - Jobs and their steps (in order)

   c. Create or update the corresponding documentation file at `docs/<workflow-name>.md` where `<workflow-name>` matches the YML filename without the extension (e.g., `docs/my-workflow.md`).

3. The documentation markdown file must follow this structure:

   ```markdown
   # <Workflow Name>

   ## Overview

   Brief description of what this workflow does.

   ## Triggers

   List and explain each trigger.

   ## Permissions

   List the permissions required.

   ## Environment Variables

   List any environment variables defined at the workflow level.

   ## Jobs

   For each job, provide:
   - Job name and runner
   - List of steps with descriptions

   ### <Job Name>

   **Runs on**: `<runner>`

   **Steps**:

   1. **<Step Name>**: Description of what this step does and why.
   2. ...
   ```

4. Make sure the `docs/` directory exists. If it does not exist, create it.

5. Write the documentation file with clear, accurate, and human-readable content.

## Important

- Preserve existing documentation for workflows that were NOT changed.
- If a workflow YML is removed, leave its documentation in place unless explicitly told to remove it.
- Use the actual content of the YML file to generate accurate descriptions.
- Describe each step in plain language, explaining what it does and its purpose in the workflow.
