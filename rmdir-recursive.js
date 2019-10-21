/*
 * I hodgepodged this from a couple of different sources.
 ! This is negated by Node 12.10 or something like that as there's a recursive flag on fs.rmdir()
 */

const fs = require('fs');
const path = require('path');

const mkdirAsync = util.promisify(fs.mkdir);
const unlinkAsync = util.promisify(fs.unlink);
const readdirAsync = util.promisify(fs.readdir);
const rmdirAsync = util.promisify(fs.rmdir);

async function rmdirRecursive(root) {
    const entries = await readdirAsync(root, { withFileTypes: true });
    const unlinkPromises = entries.map(async entry => {
        const child = path.join(root, `${entry.name}`);
        if (entry.isDirectory()) {
            await rmdirRecursive(child);
            await rmdirAsync(child);
        } else {
            await unlinkAsync(child);
        }
    });
    await Promise.all(unlinkPromises);
}
