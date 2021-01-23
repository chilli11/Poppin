import { helper } from '@ember/component/helper';

function stringReplace([str], { target, replacement }) {
  if (!str || !target || !replacement) return str;
  return str.replace(target, replacement);
}

export default helper(stringReplace);
