# Goldbug24

DefCon 2024 CPV Goldbug Puzzles

[The Puzzles](https://bbs.goldbug.cryptovillage.org/puzzles.html)

## Etiquette

### PdfToImages

Takes the source pdf and converts it into a series of image files, library used has a dependency on GhostScript [here](https://community.chocolatey.org/packages/Ghostscript) being installed

### ImageHighlighter

Takes the output from PdfToImages and uses ImageSharp to highlight pixels close to a certain colour to identify folds

### ImageStitcher

Creates a composite image from predefined columns of the output of ImageHighlighter - to visualise the aggregate folded pages - revealing the word

## Masquerade

### PerformDance

Executes the dance as instructed in [here](https://bbs.goldbug.cryptovillage.org/puzzles/2024/Masquerade/ADustyTome.txt)
