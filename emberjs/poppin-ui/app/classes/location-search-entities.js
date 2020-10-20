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
		if ((!params.geo || !params.geo.coordinates || !params.geo.coordinates.length) && !params.location) {
			throw '`location` or `geo.coordinates` required.'
		}
		this.term = params.term;
		this.location = params.location;
		if (params.geo && params.geo.coordinates && params.geo.coordinates.length)
			this.geo = params.geo;
		this.radius = !!params.radius || 40000;
		this.categories = params.categories || [];
	}
}