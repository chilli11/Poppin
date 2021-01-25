import { helper } from '@ember/component/helper';

/**
 * @param {String[1]} arguments
 * @param {Object} hash
 * @param {String} hash.target
 * @param {String} hash.replacement
 * @returns {String}
 */
function stringReplace([str], { target, replacement }) {
  if (!str || !target || !replacement) return str;
  return str.replace(target, replacement);
}

export default helper(stringReplace);
