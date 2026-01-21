# GitHub Guide for Complete Beginners

This guide assumes you know **absolutely nothing** about GitHub or Git. We'll walk through everything step-by-step.

---

## Table of Contents

1. [What is GitHub?](#what-is-github)
2. [Setting Up GitHub](#setting-up-github)
3. [Getting the Project on Your Computer](#getting-the-project-on-your-computer)
4. [Understanding the Basics](#understanding-the-basics)
5. [Your Daily Workflow](#your-daily-workflow)
6. [Common Tasks Explained](#common-tasks-explained)
7. [Troubleshooting](#troubleshooting)
8. [Visual Guide: What Each Command Does](#visual-guide-what-each-command-does)

---

## What is GitHub?

**GitHub** is like Google Drive for code, but smarter. It:
- Stores all versions of your project files
- Lets multiple people work on the same project without overwriting each other
- Keeps a history of every change made
- Lets you go back to any previous version if something breaks

**Think of it like this:**
- **GitHub.com** = The cloud storage (like Dropbox)
- **Git** = The tool on your computer that talks to GitHub
- **Repository (repo)** = Your project folder on GitHub

---

## Setting Up GitHub

### Step 1: Create a GitHub Account

1. Go to [github.com](https://github.com)
2. Click "Sign up"
3. Choose a username, enter your email, and create a password
4. Verify your email address

### Step 2: Install Git on Your Computer

**Windows:**
1. Download Git from [git-scm.com/download/win](https://git-scm.com/download/win)
2. Run the installer
3. Use all default settings (just keep clicking "Next")
4. When it asks about "Git from the command line", choose "Git from the command line and also from 3rd-party software"

**Mac:**
1. Open Terminal (search for it in Spotlight)
2. Type: `git --version`
3. If it says "command not found", install Xcode Command Line Tools by typing: `xcode-select --install`

**Linux:**
- Ubuntu/Debian: `sudo apt-get install git`
- Fedora: `sudo dnf install git`

### Step 3: Install Git LFS (Large File Storage)

**Why?** Our project uses large files (images, audio, 3D models). Git LFS handles these properly.

**Windows:**
1. Download from [git-lfs.github.com](https://git-lfs.github.com)
2. Run the installer
3. Open PowerShell or Command Prompt
4. Type: `git lfs install`
5. You should see "Git LFS initialized"

**Mac:**
1. Install Homebrew if you don't have it: `/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"`
2. Type: `brew install git-lfs`
3. Then: `git lfs install`

**Linux:**
- Ubuntu/Debian: `sudo apt-get install git-lfs && git lfs install`
- Fedora: `sudo dnf install git-lfs && git lfs install`

### Step 4: Tell Git Who You Are

Open PowerShell (Windows) or Terminal (Mac/Linux) and type:

```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

Use the **same email** you used for your GitHub account!

---

## Getting the Project on Your Computer

### Option A: Clone the Repository (First Time)

**"Clone"** means "download the entire project from GitHub to your computer."

1. Go to the project's GitHub page (someone will give you the link)
2. Click the green **"Code"** button
3. Click the **copy icon** next to the URL (it looks like two overlapping squares)
4. Open PowerShell or Terminal on your computer
5. Navigate to where you want the project:
   ```bash
   cd Documents
   # or wherever you want it
   ```
6. Type:
   ```bash
   git clone [paste the URL you copied]
   ```
   Example: `git clone https://github.com/username/project-name.git`
7. Press Enter
8. Wait for it to download (this might take a while for large projects)
9. Navigate into the project folder:
   ```bash
   cd project-name
   ```

**You now have the project on your computer!**

### Option B: You Already Have It

If someone gave you a folder with the project already in it:
1. Open PowerShell/Terminal
2. Navigate to the project folder:
   ```bash
   cd "path/to/your/project"
   ```
3. Type: `git status`
4. If it shows information about files, you're good!
5. If it says "not a git repository", see [Troubleshooting](#troubleshooting)

---

## Understanding the Basics

### The Three States of Files

Files in your project can be in three states:

1. **Untracked** = New file, Git doesn't know about it yet
2. **Modified** = You changed an existing file
3. **Staged** = You've marked it to be saved in the next "commit"

### The Three Main Areas

1. **Working Directory** = Your actual files on your computer (what you see in Unity)
2. **Staging Area** = Files you've marked to save (using `git add`)
3. **Repository** = Saved versions of your work (on GitHub and locally)

### Key Concepts

- **Commit** = A saved snapshot of your work (like a save point in a game)
- **Push** = Upload your commits to GitHub (share with team)
- **Pull** = Download other people's commits from GitHub (get their work)
- **Branch** = A separate timeline of changes (like an alternate universe)
- **Merge** = Combine changes from different branches

---

## Your Daily Workflow

### The Golden Rule: Always Pull Before You Start

**Every single day, before you do ANY work:**

1. Open PowerShell/Terminal
2. Navigate to your project folder:
   ```bash
   cd "path/to/your/project"
   ```
3. Type: `git pull`
4. Wait for it to finish
5. **Now** you can open Unity and start working

**Why?** This gets all the latest changes from your teammates. If you don't do this, you might overwrite their work!

### The 5-Step Daily Routine

**Step 1: Pull Latest Changes**
```bash
git pull
```
*Gets everyone else's work from GitHub*

**Step 2: Do Your Work**
*Open Unity, create art, write code, etc.*

**Step 3: Check What Changed**
```bash
git status
```
*Shows you which files you modified*

**Step 4: Save Your Work (Commit)**
```bash
git add .
git commit -m "Brief description of what you did"
```
*Example: `git commit -m "Added office background art"`*

**Step 5: Share Your Work (Push)**
```bash
git push
```
*Uploads your work to GitHub so others can see it*

---

## Common Tasks Explained

### Task 1: See What Files You Changed

```bash
git status
```

**What it shows:**
- Files in **red** = Modified but not staged
- Files in **green** = Staged and ready to commit
- Files not listed = No changes

### Task 2: See What Changed in a File

```bash
git diff filename
```

**Example:** `git diff MyScript.cs`

Shows you line-by-line what you changed (additions in green, deletions in red).

### Task 3: Save Your Work (Commit)

**Step 1: Stage your files**
```bash
git add .
```
*The `.` means "all changed files"*

Or stage specific files:
```bash
git add Assets/Art/Background.png
git add Assets/Scripts/PlayerController.cs
```

**Step 2: Commit with a message**
```bash
git commit -m "Clear description of what you did"
```

**Good commit messages:**
- ✅ "Add office background art"
- ✅ "Fix enemy patrol path bug"
- ✅ "Update dialogue for Scene 2"
- ❌ "stuff"
- ❌ "changes"
- ❌ "fix"

### Task 4: Share Your Work (Push)

```bash
git push
```

**If it asks for username/password:**
- Username = Your GitHub username
- Password = Use a **Personal Access Token** (not your GitHub password)

**How to create a Personal Access Token:**
1. Go to GitHub.com → Your profile picture → Settings
2. Scroll down to "Developer settings"
3. Click "Personal access tokens" → "Tokens (classic)"
4. Click "Generate new token (classic)"
5. Give it a name like "My Computer"
6. Check "repo" (gives access to repositories)
7. Click "Generate token"
8. **Copy the token immediately** (you won't see it again!)
9. Use this token as your password when Git asks

### Task 5: Get Teammates' Work (Pull)

```bash
git pull
```

**What happens:**
- Downloads changes from GitHub
- Tries to merge them with your local changes
- If there are conflicts, Git will tell you (see Troubleshooting)

### Task 6: Undo Changes to a File

**Haven't committed yet?**
```bash
git checkout -- filename
```
*Reverts the file to the last committed version*

**Example:** `git checkout -- Assets/Scripts/MyScript.cs`

**Already committed?**
```bash
git revert HEAD
```
*Creates a new commit that undoes the last commit*

### Task 7: Create a Branch (Work in Isolation)

**When to use:** Working on a big feature that will take days

```bash
git checkout -b feature/my-feature-name
```

**Example:** `git checkout -b feature/enemy-ai`

**Switch back to main branch:**
```bash
git checkout main
```

**Switch to your branch again:**
```bash
git checkout feature/my-feature-name
```

**Push your branch to GitHub:**
```bash
git push -u origin feature/my-feature-name
```

### Task 8: Temporarily Save Work (Stash)

**When to use:** You're not ready to commit, but need to pull or switch branches

```bash
git stash
```
*Saves your changes temporarily*

**Get your work back:**
```bash
git stash pop
```

---

## Troubleshooting

### Problem: "fatal: not a git repository"

**What it means:** You're not in a Git project folder.

**Solution:**
1. Make sure you're in the project folder
2. Check if there's a `.git` folder (it might be hidden)
3. If you don't have the project, use `git clone` (see [Getting the Project](#getting-the-project-on-your-computer))

### Problem: "Please tell me who you are"

**What it means:** Git doesn't know your name/email.

**Solution:**
```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

### Problem: "Your branch is ahead of 'origin/main' by X commits"

**What it means:** You have commits that aren't on GitHub yet.

**Solution:** Just run `git push` to upload them.

### Problem: "Your branch is behind 'origin/main' by X commits"

**What it means:** GitHub has changes you don't have.

**Solution:** Run `git pull` to get them.

### Problem: Merge Conflicts

**What it means:** You and a teammate changed the same part of the same file.

**What to do:**
1. **Don't panic!** This is normal.
2. Git will mark the conflicts in the file with `<<<<<<<`, `=======`, and `>>>>>>>`
3. Open the file in a text editor
4. You'll see something like:
   ```
   <<<<<<< HEAD
   Your version of the code
   =======
   Their version of the code
   >>>>>>> branch-name
   ```
5. Decide which version to keep, or combine both
6. Delete the `<<<<<<<`, `=======`, and `>>>>>>>` lines
7. Save the file
8. Stage the fixed file: `git add filename`
9. Complete the merge: `git commit`

**For Unity scene/prefab conflicts:**
- These are tricky! Coordinate with teammates
- Consider using "theirs" if you haven't made changes: `git checkout --theirs filename`
- Or "yours" if you're sure: `git checkout --ours filename`
- Always test after resolving conflicts!

### Problem: "Authentication failed" or "Permission denied"

**What it means:** Git can't log into GitHub.

**Solutions:**
1. Make sure you're using a Personal Access Token (not your password)
2. Check your username is correct
3. Try: `git config --global credential.helper store` (saves your credentials)

### Problem: "Large files detected" or "file is too large"

**What it means:** You tried to commit a file that's too big for regular Git.

**Solution:**
1. Make sure Git LFS is installed: `git lfs version`
2. Make sure it's initialized: `git lfs install`
3. The `.gitattributes` file should handle this automatically
4. If a file was already committed before LFS was set up, you may need help from a team lead

### Problem: "Cannot lock ref" or "Another git process seems to be running"

**What it means:** Git thinks another Git command is running.

**Solution:**
1. Close Unity (it might be using Git)
2. Wait a few seconds
3. Try again
4. If it persists, delete the `.git/index.lock` file (if it exists)

### Problem: Unity shows "Missing Script" or pink materials

**What it means:** You moved/renamed files outside Unity.

**Solution:**
1. **Never move or rename Unity assets outside Unity!**
2. Always use Unity's Project window
3. If you already did it, undo in Git: `git checkout -- Assets/`
4. Then move/rename inside Unity

### Problem: "Everything is deleted" after pull

**What it means:** You might have pulled while on the wrong branch, or there was a merge issue.

**Solution:**
1. **Don't panic!** Your work is probably still there
2. Check what branch you're on: `git branch`
3. Check recent commits: `git log --oneline -10`
4. You can go back: `git checkout [commit-hash]`
5. If you're really stuck, ask for help immediately!

---

## Visual Guide: What Each Command Does

### `git status`
```
Shows you:
- What files you changed
- What's staged to commit
- What branch you're on
```

### `git add .`
```
Takes all your changed files
and marks them as "ready to save"
```

### `git commit -m "message"`
```
Takes all staged files
and saves them as a snapshot
(like a save point in a game)
```

### `git push`
```
Takes your local commits
and uploads them to GitHub
(so teammates can see your work)
```

### `git pull`
```
Downloads commits from GitHub
and merges them with your local work
(gets teammates' changes)
```

### `git checkout -b branch-name`
```
Creates a new timeline
where you can work in isolation
(like an alternate universe)
```

### `git stash`
```
Temporarily saves your work
so you can switch branches or pull
(like a temporary save slot)
```

---

## Quick Reference Card

**Before starting work:**
```bash
git pull
```

**After making changes:**
```bash
git status                    # See what changed
git add .                     # Mark files to save
git commit -m "Description"   # Save your work
git push                      # Share with team
```

**Emergency commands:**
```bash
git checkout -- filename      # Undo changes to a file
git stash                     # Temporarily save work
git stash pop                 # Get stashed work back
git log --oneline -10         # See recent commits
```

---

## Getting Help

**If you're stuck:**
1. Read this guide again (especially the Troubleshooting section)
2. Check [CONTRIBUTING.md](../CONTRIBUTING.md) for workflow details
3. Ask a teammate or team lead
4. Google the error message (Git errors are usually well-documented)

**Remember:** Everyone was a beginner once. It's okay to ask questions!

---

## Next Steps

Once you're comfortable with the basics:
- Read [CONTRIBUTING.md](../CONTRIBUTING.md) for detailed coding standards
- Learn about branching strategies for larger features
- Explore GitHub's web interface for viewing changes and history

**You've got this!** 🚀
