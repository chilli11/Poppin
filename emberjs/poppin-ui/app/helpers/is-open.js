import { helper } from '@ember/component/helper';

/**
 * Determines if business is open
 * @param {LocationModel} params 
 * @return {bool}
 */
const isOpen = function isOpen(params) {
  const location = params[0];
  if (location.hours) {
    const today = new Date();
    var day = today.getDay();
    var hour = today.getHours();
    var minute = today.getMinutes();

    if (day == 0) day = 6;
    else day--;
    let todayHours = location.hours[day];

    if (!todayHours.opening || !todayHours.closing) return false;

    let opening = parseInt(todayHours.opening.substr(0,2), 10);
    let closing = parseInt(todayHours.closing.substr(0,2), 10);

    if (opening <= hour)
    {
      if (closing < opening || closing > hour) {
        return true;
      } else {
        return false;
      }
    } else {
      if (day == 0) day = 6;
      else day--;
      todayHours = location.hours[day];
      if (!todayHours.opening || !todayHours.closing) return false;
      opening = parseInt(todayHours.opening.substr(0,2), 10);
      closing = parseInt(todayHours.closing.substr(0,2), 10);
      if (closing < opening && closing > hour) {
        return true;
      } else {
        return false;
      }
    }
  }
}

export default helper(isOpen);
