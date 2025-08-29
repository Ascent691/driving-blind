# Driving Blind

Two friends, Jack and Lee, are challenged to drive a vehicle around an obstacle course. The course is divided into a grid and the objective of the challenge is for Jack to drive the vehicle while receiving instructions from Lee on which cell to move to from his current position. 

The course is setup such that each cell in the grid can contain nothing (an empty cell), a wall or a hazard. If an instruction would make Jack drive into a wall, or outside of the course, Jack can safely ignore the instruction. If an instruction would make Jack drive into a hazard the attempt is ended and Jack will not be allowed to follow any further instructions.

Some of the empty cells in the grid have been marked as interesting cells. An interesting cell can either be an interesting start or an interesting finish cell. A pair of both an interesting start and interesting finish is considered drivable if there is a strategy to drive the car through walkie-talkie instructions from the start that makes it end at the finish 99% of the time.

Lee can ask Jack to move in four directions, north, south, east and west. Jack can only move 1 cell per instruction.

However, Jack finds it very difficult to understand the instructions from Lee when speaking over the walkie-talkie, as a result, 50% of the time Jack receives an instruction to move east, Jack mistakenly moves west, similarly for north and south instructions, 50% of the time Jack misunderstands and drives to the opposite cell.

Find all the drivable pairs.

## Input

The first line of the input gives the number of test cases, T. T test cases follow. Each test case starts with a line containing two integers R and C, the number of rows and columns of the grid. Then, R lines follow containing a string of C characters each. The j-th character on the i-th of these lines Gij represents the grid in the i-th row and j-th column as follows:

- A period (.) represents an uninteresting empty cell.
- A hash symbol (#) represents a cell containing a wall.
- A asterisk (*) represents a cell containing a hazard.
- A lowercase letter (a-z) represents an interesting start.
- A uppercase letter (A-Z) represents an interesting finish.

## Output

For each test case, output one line containing Case #x: y, where x is the test case number (starting from 1) and y is NONE if there are no drivable pairs. Otherwise, y must be a series of 2 character strings separated by spaces, representing all drivable pairs with start letter first and the finish letter second, in alphabetical order.


## Limits

Time limit: 60 seconds