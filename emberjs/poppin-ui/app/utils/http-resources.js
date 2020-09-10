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
	incrementCrowd: {
		url: 'locations/incrementCrowd/:locId',
		method: GET,
		params: ['locId']
	},
	decrementCrowd: {
		url: 'locations/decrementCrowd/:locId',
		method: GET,
		params: ['locId']
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
	isAuthenticated: {
		url: 'identity/is-authenticated',
		method: GET
	},
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
	},
	myProfile: {
		url: 'profile',
		method: GET
	},

	/* ===== VENDOR ===== */
	createNewVendor: {
		url: 'vendors',
		method: POST
	},
	updateVendor: {
		url: 'vendors',
		method: PUT
	},
	getAllVendors: {
		url: 'vendors',
		method: GET
	},
	getVendorById: {
		url: 'vendors/:vendorId',
		method: GET
	},
	getVendorsByList: {
		url: 'vendors/get-by-list',
		method: POST
	},
	getVendorsBySearch: {
		url: 'vendors/get-by-search',
		method: POST
	},
	addMember: {
		url: 'vendors/add-member/:vendorId',
		method: POST,
		params: ['vendorId']
	},
	removeMember: {
		url: 'vendors/remove-member/:vendorId',
		method: POST,
		params: ['vendorId']
	}
};

export default HttpResources;
