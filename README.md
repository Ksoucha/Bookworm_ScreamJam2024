# Bookworm_ScreamJam2024

## Game Overview
 A horror game made with Unity in first-person 3D, in collaboration with 6 people listed here: https://ksoucha.itch.io/the-bookworm. 
 
<p align="justify">
The game idea came from me: I tried to make a wordplay with the word "bookworm", first by taking it figuratively (a person who loves reading books), and then literally (a worm living inside a book). The game starts with multiple images and text used to tell the story of a girl who spends her days reading, when one day she finishes her book and heads to the library. The player then can move around to head to the library. When the player is
in the library, the screen turns black and one of the books "devours" him. The player ends up in a maze, and he has to escape before the bookworm "eats" him. There will be a white flag indicating the maze exit. While the  bookworm is flying above the maze, the floor and wall tiles that he passes by will disappear, so the player has to hurry. The bookworm will also occasionnally make scary sounds. The player has a dim torch lamp to guide him through the maze. The bad ending will be displayed if the player loses, and the same goes for the good ending. 
</p>

## Features
* Maze with an AI
  * The AI flies randomly across the maze (levitating in the air)
  * The AI destroys floor and wall tiles after passing above them
  * The AI occasionnally emits unsettling sounds
* UI-driven narrative that transitions automatically after a few seconds through text and images
* Includes seed-based maze generation for customizable layouts

## Bugs to fix
* Player movement is not smooth (mouse sensitivity is too high and camera rotation is not gradual)
* The AI is moving too slowly and sometimes the wall tiles do not disappear
* The AI sometimes gets stuck and does not move anymore
* The sounds that the AI is supposed to make are not always perceptible
