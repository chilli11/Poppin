import Model, { attr } from '@ember-data/model';

export default class LocationModel extends Model {
  @attr('string') yelpId;
  @attr('string') name;
  @attr('string') phone;
	@attr address;
	@attr('string') logoUrl;
	@attr('string') mainPhotoUrl;
	@attr addlPhotoUrls;
	@attr('string') website;
	@attr menus;
	@attr('string') yelpUrl;
  @attr categories;
	@attr('number') capacity;
	@attr('boolean') capacityConfirmed;
  @attr('number') crowdSize;
	@attr('number') visitLength;
	@attr('number') waitTime;
	@attr('number') price;
	@attr('number') rating
	@attr hours;
  @attr('date') lastUpdate;
  @attr('boolean') atCapacity
  @attr yelpDetails;
}
