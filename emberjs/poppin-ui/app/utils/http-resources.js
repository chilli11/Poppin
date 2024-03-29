const GET = 'GET';
const POST = 'POST';
const PUT = 'PUT';
const DELETE = 'DELETE';

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
	getLocationsBySearch: {
		url: 'locations/search',
		method: POST
	},
	incrementCrowd: {
		url: 'locations/increment-crowd/:locId',
		method: GET,
		params: ['locId']
	},
	decrementCrowd: {
		url: 'locations/decrement-crowd/:locId',
		method: GET,
		params: ['locId']
	},
	checkin: {
		url: 'locations/checkin/:locId',
		method: GET,
		params: ['locId']
	},
	addFavorite: {
		url: 'profile/favorites/add/:locId',
		method: GET,
		params: ['locId']
	},
	removeFavorite: {
		url: 'profile/favorites/remove/:locId',
		method: GET,
		params: ['locId']
	},

	/* ===== CATEGORIES ===== */
	getCategories: {
		url: 'categories',
		method: GET
	},
	addCategory: {
		url: 'categories',
		method: POST
	},
	updateCategory: {
		url: 'categories',
		method: PUT
	},
	deleteCategory: {
		url: 'categories/:slug',
		method: DELETE,
		params: ['slug']
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
	updateProfile: {
		url: ['profile'],
		method: PUT
	},
	confirmEmail: {
		url: 'identity/confirm-email/:userId',
		method: GET,
		params: ['userId']
	},
	resendConfirmationEmail: {
		url: 'identity/resend-confirmation',
		method: POST
	},
	passwordResetRequest: {
		url: 'identity/forgot-password',
		method: POST
	},
	confirmResetToken: {
		url: 'identity/reset-password/:userId',
		method: GET,
		params: ['userId']
	},
	resetPassword: {
		url: 'identity/reset-password/:userId',
		method: POST,
		params: ['userId']
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
	getMyVendors: {
		url: 'vendors',
		method: GET
	},
	getAllVendors: {
		url: 'vendors/all',
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
	},
	addVendorLocation: {
		url: 'vendors/add-location/:vendorId',
		method: POST,
		params: ['vendorId']
	},
	removeVendorLocation: {
		url: 'vendors/remove-location/:vendorId',
		method: POST,
		params: ['vendorId']
	},

	/* ===== OAUTH ===== */
	authorizeFacebook: {
		url: 'identity/facebook-login',
		method: POST
	}
};

export default HttpResources;
