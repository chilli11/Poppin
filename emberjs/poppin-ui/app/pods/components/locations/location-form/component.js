import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action } from '@ember/object';
import { inject as service } from '@ember/service';

import { states, actions } from './constants';

const defHours = {
	Sunday: { opening: null, closing: null },
	Monday: { opening: null, closing: null },
	Tuesday: { opening: null, closing: null },
	Wednesday: { opening: null, closing: null },
	Thursday: { opening: null, closing: null },
	Friday: { opening: null, closing: null },
	Saturday: { opening: null, closing: null }
};

export default class LocationFormComponent extends StatefulComponent {
	namespace = 'LocationForm';
	@service locationsService;
	@service yelpService;
	@service store;

	transitions = {
		[states.IDLE]: {
			[actions.SUBMIT_LOCATION]: states.SUBMITTING_LOCATION,
			[actions.GET_FULL_MATCH]: states.GETTING_FULL_MATCH,
			[actions.SUBMIT_MATCH]: states.SUBMITTING_MATCH
		},
		[states.SUBMITTING_LOCATION]: {
			[actions.GET_MATCHES]: states.GETTING_MATCHES,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.GETTING_MATCHES]: {
			[actions.RESOLVE_GET_MATCHES]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.GETTING_FULL_MATCH]: {
			[actions.RESOLVE_FULL_MATCH]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.SUBMITTING_MATCH]: {
			[actions.RESOLVE_SUBMIT_MATCH]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		}
	};

	@tracked yelpMatches;
	@tracked locationId;
	@tracked name;
	@tracked yelpId;
	@tracked addressLine1;
	@tracked addressLine2;
	@tracked city;
	@tracked state;

	@tracked zip;	
	get zipCode() {
		return this.zip ? this.zip.substr(0, 5) : null;
	}
	get zipCodeTrailing() {
		const spl = this.zip ? this.zip.split('-') : [];
		return spl[1] || null;
	}

	@tracked coordinates;
	@tracked capacity = '0';
	@tracked crowdSize = '0';
	@tracked hours = defHours;

	get locationDTO() {
		const { locationId, name, yelpId,  capacity, crowdSize, hours } = this;
		return  {
			id: locationId,
			yelpId: yelpId,
			name,
			address: {
				line1: this.addressLine1,
				line2: this.addressLine2,
				city: this.city,
				state: this.state,
				zipCode: parseInt(this.zipCode, 10),
				zipCodeTrailing: this.zipCodeTrailing ? parseInt(this.zipCodeTrailing) : null,
				coordinates: this.coordinates
			},
			categories: [],
			capacity: parseInt(capacity, 10),
			crowdSize: parseInt(crowdSize, 10),
			hours
		};
	}

	get canAcceptMatch() {
		return !!this.locationId;
	}

	constructor() {
		super(...arguments);
		this.populateFromPoppin();
		this.initMachine();
	}

	@action
	clearForm() {
		this.locationId = null;
		this.yelpId = null;
		this.name = null;
		this.addressLine1 = null;
		this.addressLine2 = null;
		this.city = null;
		this.state = null;
		this.zipCode = null;
		this.zipCodeTrailing = null;
		this.capacity = 0;
		this.hours = defHours;
	}

	populateFromPoppin(location) {
		const loc = location || this.args.location
		if (loc) {
			this.locationId = loc.id;
			this.yelpId = loc.yelpId;
			this.name = loc.Name || null;
			this.addressLine1 = loc.address.line1;
			this.addressLine2 = loc.address.line2;
			this.city = loc.address.city;
			this.state = loc.address.state;
			this.zipCode = loc.address.zipCode;
			this.zipCodeTrailing = loc.address.zipCodeTrailing;
			this.capacity = loc.capacity || 0;
			this.hours = loc.Hours || defHours;
		}
	}

	populateFromYelp(loc) {		
		this.yelpId = loc.id;
		this.name = loc.name || null;
		this.addressLine1 = loc.location.address1;
		this.addressLine2 = loc.location.address2;
		this.city = loc.location.city;
		this.state = loc.location.state;
		this.zipCode = loc.location.zip.split('-')[0];
		this.coordinates = loc.location.coordinates;
		this.capacity = 0;
		this.hours = loc.Hours || defHours;
	}

	checkMatch(business) {
		business.isMatch = business.id == this.yelpId;
		return business;
	}

	[actions.SUBMIT_LOCATION]() {
		return this.locationsService.createNewLocation(this.locationDTO).then((location) => {
			this.locationId = location.id;
			this.yelpId = location.yelpId;
			return this.dispatch(actions.GET_MATCHES, location.id);
		}).catch(data => this.dispatch(actions.REJECT_ACTION, data));
	}

	[actions.GET_MATCHES](id) {
		return this.yelpService.getLocMatch(id).then((data) => {
			this.yelpMatches = data.businesses.map(b => this.checkMatch(b));
			return this.dispatch(actions.RESOLVE_GET_MATCHES, data);
		}).catch(data => this.dispatch(actions.REJECT_ACTION, data));
	}

	[actions.GET_FULL_MATCH](match) {
		return this.yelpService.getYelpBusiness(match.id).then((data) => {
			this.populateFromYelp(data);
			return this.dispatch(actions.RESOLVE_FULL_MATCH, data);
		}).catch(data => this.dispatch(actions.REJECT_ACTION, data));
	}

	[actions.SUBMIT_MATCH](match) {
		this.yelpId = match.id;
		return this.locationsService.updateLocation(this.locationDTO)
			.then(data => this.dispatch(actions.RESOLVE_SUBMIT_MATCH, data))
			.catch(data => this.dispatch(actions.REJECT_ACTION, data));
	}

	[actions.RESOLVE_SUBMIT_MATCH]() {
		alert('Match Added!')
	}

	[actions.REJECT_ACTION](data) {
		console.error(data);
	}


	@action
	submit() {
		return this.dispatch(actions.SUBMIT_LOCATION, null, true);
	}

	@action
	acceptMatch(match) {
		return this.dispatch(actions.SUBMIT_MATCH, match);
	}

	@action
	fillData(match) {
		this.dispatch(actions.GET_FULL_MATCH, match);
	}
}
