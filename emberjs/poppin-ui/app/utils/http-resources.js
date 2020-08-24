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
	getLocationsByYelpList: {
		url: 'locations/yelp-search',
		method: POST
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
	},

	/* ===== ACCOUNT =====*/
	registerAccount: {
		url: 'identity/register',
		method: POST
	},
	login: {
		url: 'identity/login',
		method: POST
	},
	myAccount: {
		url: 'identity/me',
		method: POST
	}
};

export default HttpResources;
