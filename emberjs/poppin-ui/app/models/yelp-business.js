import Model, { attr } from '@ember-data/model';

/**
 * @prop {String} id
 * @prop {String} alias
 * @prop {String} name
 * @prop {String} imageUrl
 * @prop {Boolean} isClaimed
 * @prop {Boolean} isClosed
 * @prop {String} url
 * @prop {String} phone
 * @prop {String} displayPhone
 * @prop {Int} reviewCount
 * @prop {Object} categories
 * @prop {String} categories.alias
 * @prop {String} categories.title
 * @prop {String[]} categories.parentAliases
 * @prop {String[]} categories.countryWhitelist
 * @prop {String[]} categories.countryBlacklist
 * @prop {Float} rating
 * @prop {Object} location
 * @prop {String} location.address1
 * @prop {String} location.address2
 * @prop {String} location.address3
 * @prop {String} location.city
 * @prop {String} location.state
 * @prop {String} location.zipCode
 * @prop {String} location.country
 * @prop {Object} coordinates
 * @prop {Float} coordinates.longitude
 * @prop {Float} coordinates.latitude
 * @prop {String[]} photos
 * @prop {String} price
 * @prop {String[]} transactions
 * @prop {Object} hours
 * @prop {Object} hours.open
 * @prop {String} hours.open.start
 * @prop {String} hours.open.end
 * @prop {Int} hours.open.day
 * @prop {Boolean} hours.open.isOvernight
 * @prop {String} hours.hoursType
 * @prop {Boolean} hours.isOpenNow
 */
export default class YelpBusinessModel extends Model {
	@attr('string') id;
	@attr('string') alias;
	@attr('string') name;
	@attr('string') imageUrl;
	@attr('boolean') isClaimed;
	@attr('boolean') isClosed;
	@attr('string') url;
	@attr('string') phone;
	@attr('string') displayPhone;
	@attr('number') reviewCount;
	@attr categories;
	@attr('number') rating;
	@attr location;
	@attr coordinates;
	@attr photos;
	@attr('string') price;
	@attr transactions;
	@attr hours;
}
