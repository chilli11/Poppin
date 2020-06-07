const GET = 'GET';
const POST = 'POST';
const PUT = 'PUT';

const HttpResources = {
	/* ===== LOCATIONS ===== */
	getLocation: {
		url: 'locations/:locId',
		method: GET,
		params: ['locId']
	},
	createNewLocation: {
		url: 'locations',
		method: POST,
	},
	updateLocation: {
		url: 'locations',
		method: PUT
	},

	/* ===== YELP =====*/
	getYelpMatch: {
		url: 'yelp/match/',
		method: POST
	},
	getYelpMatchLocId: {
		url: 'yelp/match/:locId',
		method: GET,
		params: ['locId']
	},
	getYelpBusiness: {
		url: 'yelp/:yelpId',
		method: GET,
		params: ['yelpId']
	},
	getYelpBusinessSearch: {
		url: 'yelp/businesses',
		method: POST
	}
};

export default HttpResources;
