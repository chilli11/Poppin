import { assign } from '@ember/polyfills';

/**
 *
 * @export
 * @class LocationSearchEntities
 * @prop {String} term full text search term
 * @prop {GeoJson} geo https://geojsonlint.com/
 * @prop {Float} radius in meters, max 40000
 * @prop {String[]} categories slugs
 */
export class LocationSearchRequest {
	term;
	location;
	geo = {
		type: 'Point',
		coordinates: []
	};
	radius = 40000;
	categories = [];

	constructor(params) {
		var self = this;
		if (!params.geo.coordinates.length && !params.location) {
			throw '`location` or `geo.coordinates` required.'
		}
		assign(self, params);
	}
}