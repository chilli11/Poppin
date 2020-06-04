import Model, { attr } from '@ember-data/model';

export default class LocationModel extends Model {
  // @attr('string') Id;
  @attr('string') yelpId;
  @attr('string') name;
  @attr('string') phone;
  @attr address;
  @attr categories;
  @attr('number') capacity;
  @attr('number') crowdSize;
  @attr hours;
  @attr('date') lastUpdate;
  @attr('boolean') atCapacity
  @attr yelpDetails;
}
