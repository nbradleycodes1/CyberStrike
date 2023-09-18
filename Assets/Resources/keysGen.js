const fs = require('fs');

// How much should it expand towards each possible direction
const RADIUS = 20;

// source: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/random
function getRandomInt() {
  const min = Math.ceil(Number.MIN_SAFE_INTEGER);
  const max = Math.floor(Number.MAX_SAFE_INTEGER);
  return Math.floor(Math.random() * (max - min) + min); // The maximum is exclusive and the minimum is inclusive
}

const genPieces = () => ({
    0b010000: getRandomInt(), // bA1
    0b010001: getRandomInt(), // bA2
    0b010010: getRandomInt(), // bA3
    0b010011: getRandomInt(), // bG1
    0b010100: getRandomInt(), // bG2
    0b010101: getRandomInt(), // bG3
    0b010110: getRandomInt(), // bB1
    0b010111: getRandomInt(), // bB2
    0b011000: getRandomInt(), // bS1
    0b011001: getRandomInt(), // bS2
    0b011010: getRandomInt(), // bQ1
    0b100000: getRandomInt(), // wA1
    0b100001: getRandomInt(), // wA2
    0b100010: getRandomInt(), // wA3
    0b100011: getRandomInt(), // wG1
    0b100100: getRandomInt(), // wG2
    0b100101: getRandomInt(), // wG3
    0b100110: getRandomInt(), // wB1
    0b100111: getRandomInt(), // wB2
    0b101000: getRandomInt(), // wS1
    0b101001: getRandomInt(), // wS2
    0b101010: getRandomInt(), // wQ1
});

const board = [[{x: 0, y: 0}, genPieces()]];

// const OFFSETS = [
//     // Notice how each side is only valid if it adds up to an even number
//     {x: 1, y: 1},    // [0] North
//     {x:-1, y: 1},    // [1] Northwest
//     {x: -2, y: 0},   // [2] Southwest
//     {x: -1, y: -1},  // [3] South
//     {x:  1, y: -1},  // [4] Southeast
//     {x:  2, y: 0},   // [5] Northeast
// ];

// For Unity project
const OFFSETS = [
    {x: 0, y: 56},    // [0] North
    {x: -48, y: 28},  // [1] Northwest
    {x: -48, y: -28}, // [2] Southwest
    {x: 0, y: -56},   // [3] South
    {x: 48, y: -28},  // [4] Southeast
    {x: 48, y: 28},   // [5] Northeast
]


// expand south
for (let i = 0; i < RADIUS; ++i) {
    const newelem = [{x: board[board.length - 1][0].x + OFFSETS[3].x, y: board[board.length - 1][0].y + OFFSETS[3].y}, genPieces()];
    board.push(newelem);
}

const newelem = [{x: OFFSETS[0].x, y: OFFSETS[0].y}, genPieces()];
board.push(newelem);

//expand north
for (let i = 0; i < RADIUS; ++i) {
    const newelem = [{x: board[board.length - 1][0].x + OFFSETS[0].x, y: board[board.length - 1][0].y + OFFSETS[0].y}, genPieces()];
    board.push(newelem);
}

// generate west
const west = [];
for (const pos of board) {
    const westLine = [[{x: pos[0].x + OFFSETS[2].x, y: pos[0].y + OFFSETS[2].y}, genPieces()]];
    for (let i = 0; i < RADIUS; ++i) {
        const newelem = [{x: westLine[westLine.length - 1][0].x + OFFSETS[2].x, y: westLine[westLine.length - 1][0].y + OFFSETS[2].y}, genPieces()];
        westLine.push(newelem);
    }
    west.push(westLine);
}

// generate east
const east = [];
for (const pos of board) {
    const eastLine = [[{x: pos[0].x + OFFSETS[5].x, y: pos[0].y + OFFSETS[5].y}, genPieces()]];
    for (let i = 0; i < RADIUS; ++i) {
        const newelem = [{x: eastLine[eastLine.length - 1][0].x + OFFSETS[5].x, y: eastLine[eastLine.length - 1][0].y + OFFSETS[5].y}, genPieces()];
        eastLine.push(newelem);
    }
    east.push(eastLine);
}

// expand west and east
west.map(arr => arr.map(elem => board.push(elem)));
east.map(arr => arr.map(elem => board.push(elem)));


// Output Zobrist.cs
let output = "";
output += "using System.Collections.Generic;\nnamespace HiveCore {\n \tpublic static class Zobrist {\n"
output += "\t\tpublic static Dictionary<(int, int), Dictionary<int, long>> ZOBRIST_KEYS = new Dictionary<(int, int), Dictionary<int, long>>\n";
output += "\t\t{\n";
board.map(pos => output += (`\t\t\t{ (${(pos[0].x)}, ${pos[0].y}), new Dictionary<int, long> { ${Object.entries(pos[1]).map(([piece, hash]) => `{${piece}, ${hash}}`)} } }, \n`));
output += "\t\t};\n\t}\n}";
fs.writeFileSync("./Zobrist.cs", output);


// Output zobrist.json
// let output = "{";
// board.map((pos, index) => output += (`\n\t"(${(pos[0].x)}, ${pos[0].y})": {${Object.entries(pos[1]).map(([piece, hash]) => `\n\t\t"${piece}": ${hash}`)}\n\t}${index == (board.length - 1) ? '' : ','}\n`));
// output += "}";
// fs.writeFileSync("./zobrist.json", output);