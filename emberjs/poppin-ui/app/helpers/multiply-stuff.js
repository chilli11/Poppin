import { helper } from '@ember/component/helper';

function multiplyStuff([first, second]) {
  if (!first || !second)  return null;

  if (typeof parseInt(first, 10) == 'number' && typeof parseInt(second, 10) == 'number') {
    return first * second;
  }
}
export default helper(multiplyStuff);
