#!/bin/bash

# Neural Rink - GitHub Push Script
echo "🏒 Neural Rink - GitHub Repository Setup"
echo "========================================"

# Check if we're in a git repository
if [ ! -d ".git" ]; then
    echo "❌ Error: Not in a git repository!"
    echo "Run 'git init' first."
    exit 1
fi

# Check if remote origin exists
if git remote get-url origin >/dev/null 2>&1; then
    echo "✅ Remote origin already configured:"
    git remote get-url origin
    echo ""
    echo "🚀 Pushing to GitHub..."
    git push -u origin main
else
    echo "📝 No remote repository configured yet."
    echo ""
    echo "Please create a GitHub repository first:"
    echo "1. Go to https://github.com"
    echo "2. Click 'New repository'"
    echo "3. Name it 'neural-rink'"
    echo "4. Don't initialize with README (we have one)"
    echo "5. Click 'Create repository'"
    echo ""
    echo "Then run this command with your GitHub username:"
    echo "git remote add origin https://github.com/YOUR_USERNAME/neural-rink.git"
    echo "git push -u origin main"
    echo ""
    echo "Or edit this script and add your GitHub username below:"
    echo "Edit the GITHUB_USERNAME variable and run again."
    echo ""
    
    # Uncomment and set your GitHub username here:
    # GITHUB_USERNAME="your-github-username"
    # if [ ! -z "$GITHUB_USERNAME" ]; then
    #     echo "🔗 Adding remote origin..."
    #     git remote add origin https://github.com/$GITHUB_USERNAME/neural-rink.git
    #     git branch -M main
    #     git push -u origin main
    # fi
fi

echo ""
echo "🎮 Repository Contents:"
echo "- Unity 6 project with ML-Agents"
echo "- Trained AI model (270.6 reward)"
echo "- Complete documentation"
echo "- Python training scripts"
echo "- 398 files, 43,885 lines of code"
echo ""
echo "✨ Your Neural Rink project is ready for GitHub!"
