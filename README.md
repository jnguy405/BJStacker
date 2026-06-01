# BJStacker

A physics stacker: pieces slide side to side, drop on left click, and the tower can lean until something falls.

## Scripts

| Script | Role |
|--------|------|
| `StackGameController` | Loop, score, game over |
| `PieceSpawner` | Spawns prefabs |
| `ActiveMovingPiece` | Side-to-side motion + drop |
| `StackPiece` | Physics, settle detection, tilt |
| `CameraFollowStack` | Camera rises with stack |
| `StackHeightUI` | Optional height label |
