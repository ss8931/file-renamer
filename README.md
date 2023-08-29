# File Renamer

MIT. See LICENSE.

An LLM-based file renamer.

## Motivation

I organize my files in a tree structure. So the filename itself actually needs to be short. The multiple directories and subdirectories provide the context.

For example, a file named `slide-week-01' would be in a directory: `fall/COMP-XX/slides/`. The filename might have redundant information, like the course name, or the semester.

The problem I run into is I often download files which have long names. I want to rename them to something short and meaningful. I want to do this quickly, without having to think too much about it.

LLM makes this possible. 


## Set up

Prereq. Dotnet.

Create an .env file anywhere.

Apply for a key, if you don't have one already [Google Makersuite](https://developers.generativeai.google/products/makersuite)

```
API_KEY=YOUR_KEY
PROMPT_FILE=REAL_PATH_TO_PROMPT_FILE
```

## Usage

### Training

Train the model a new mapping.

```bash
dotnet run -- add original_name new_name
```

### Renaming

Will rename the files. If there are conflicts in the generated names, or new file conflicts with an existing one, will fail.

```bash
dotnet run -- rename [files]
```

Run it in dry-run mode to see what it would do.

```bash
dotnet run -- rename --dry-run [files]
```

Or, add verbosity

```bash
dotnet run -- rename --verbose [files]
```

Or, hide files which are already named well.

```bash
dotnet run -- rename --hide-no-changes [files]
```

Or, train it. Training implies dry-run.

```bash
dotnet run -- rename --train [files]
```
