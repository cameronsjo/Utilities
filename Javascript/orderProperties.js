/*
 * This was also hodgepodged from a couple of different sources.
 */
 
/**
 * Sorts an object's properties.
 * @param {Object} Object with properties to be sorted.
 * @returns {Object} Returns an object with properties, and child-objects sorted alphabetically. 
 */
function orderProperties(o) {
    return Object.keys(o)
        .sort()
        .reduce((r, k) => {
            if (typeof o[k] === 'object' && !Array.isArray(o[k])) {
                o[k] = orderProperties(o[k]);
            }
            r[k] = o[k];
            return r;
        }, {});
}
