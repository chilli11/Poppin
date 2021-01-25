/**
 *
 * @export
 * @class LocationSearchEntities
 * @prop {String} term full text search term
 * @prop {GeoJson} geo https://geojsonlint.com/
 * @prop {Float} radius in meters, max 40000
 * @prop {String[]} categories slugs
 * @prop {Int} [pageLength] defaults to 20
 * @prop {int} [offset] defaults to 0
 */
export class LocationSearchRequest {
	term;
	location;
	categories = [];
	geo = {
		type: 'Point',
		coordinates: []
	};
	radius = 40000;
	pageLength = 20;
	offset = 0;

	constructor(params) {
		if ((!params.geo || !params.geo.coordinates || !params.geo.coordinates.length) && !params.location) {
			throw '`location` or `geo.coordinates` required.'
		}
		this.term = params.term;
		this.location = params.location;
		this.categories = params.categories || [];
		if (params.geo && params.geo.coordinates && params.geo.coordinates.length)
			this.geo = params.geo;
		this.radius = params.radius || 40000;
		this.pageLength = params.pageLength || 20;
		this.offset = params.offset || 0;
	}
}