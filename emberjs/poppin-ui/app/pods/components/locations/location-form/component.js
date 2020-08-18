import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';
import _ from 'lodash';

import { states, actions } from './constants';

const days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
const defHours = [
	{ opening: null, closing: null,  day: days[0] },
	{ opening: null, closing: null,  day: days[1] },
	{ opening: null, closing: null,  day: days[2] },
	{ opening: null, closing: null,  day: days[3] },
	{ opening: null, closing: null,  day: days[4] },
	{ opening: null, closing: null,  day: days[5] },
	{ opening: null, closing: null,  day: days[6] },
];


export default class LocationFormComponent extends StatefulComponent {
	namespace = 'LocationForm';
	@service locationsService;
	@service yelpService;
	@service store;

	days = days;

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
		return this.zip ? this.zip.toString().substr(0, 5) : null;
	}

	@tracked coordinates;
	@tracked capacity = '0';
	@tracked crowdSize = '0';
	@tracked hours = _.merge(defHours);

	@tracked modalTitle;
	@tracked modalText;
	@tracked showModal = false;

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
		this.capacity = 0;
		this.hours = _.merge(defHours);
	}

	populateFromPoppin(location) {
		const loc = location || this.args.location
		if (loc) {
			this.locationId = loc.id;
			this.yelpId = loc.yelpId;
			this.name = loc.name;
			this.addressLine1 = loc.address.line1;
			this.addressLine2 = loc.address.line2;
			this.city = loc.address.city;
			this.state = loc.address.state;
			this.zip = loc.address.zipCode;
			this.capacity = loc.capacity;
			this.hours = loc.hours || _.merge(defHours);
		}
	}

	populateFromYelp(loc) {		
		this.yelpId = loc.id;
		this.name = loc.name;
		this.addressLine1 = loc.location.address1;
		this.addressLine2 = loc.location.address2;
		this.city = loc.location.city;
		this.state = loc.location.state;
		this.zip = loc.location.zip || this.zip;
		this.coordinates = loc.location.coordinates;
		this.capacity = 0;
		
		const _hours = _.merge(defHours);
		if (loc.hours && loc.hours.length) {
			const { open } = loc.hours[0];
			open.forEach(v => {
				var obj = _hours[v.day];
				set(obj, 'opening', v.start.slice(0,2) + ':' + v.start.slice(2));
				set(obj, 'closing', v.end.slice(0,2) + ':' + v.end.slice(2));
			});
		}		
		this.hours = _hours;
	}

	checkMatch(business) {
		business.isMatch = business.id == this.yelpId;
		return business;
	}

	[actions.SUBMIT_LOCATION]() {
		const method = this.locationId ? 'updateLocation' : 'createNewLocation';
		return this.locationsService[method](this.locationDTO).then((location) => {
			if (this.locationId) {
				this.store.findRecord('location', this.locationId)
					.then((loc) => Object.keys(loc).forEach(k => loc[k] = location[k]));
			} else {
				this.store.createRecord('location', location);
			}
			this.locationId = location.id;
			this.yelpId = location.yelpId;
			if (typeof this.args.resolveAction == 'function') {
				return this.args.resolveAction(this.locationDTO);
			}
			if (location.yelpId) {
				return this.args.redirectToLocation(location.id);
			}
			
			this.modalText = this.name + " has been added to Poppin!";
			this.modalTitle = "Success!";
			this.showModal = true;
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
		return this.locationsService.updateLocation(this.locationDTO).then(data => {
			if (typeof this.args.resolveAction == 'function') {
				return this.args.resolveAction(this.locationDTO);
			}
			return this.dispatch(actions.RESOLVE_SUBMIT_MATCH, data);
		}).catch(data => this.dispatch(actions.REJECT_ACTION, data));
	}

	[actions.RESOLVE_SUBMIT_MATCH](match) {
		this.modalText = "Matched to " + match.name + ".";
		this.modalTitle = "Match Added!";
		this.showModal = true;
		return true;
	}

	[actions.REJECT_ACTION](data) {
		this.modalText = data.toString();
		this.modalTitle = "Error!";
		this.showModal = true;
		return console.error(data);
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