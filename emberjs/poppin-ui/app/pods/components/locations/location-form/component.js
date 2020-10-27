import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set, computed } from '@ember/object';
import { A as eArray } from '@ember/array';
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
	@service categoriesService;
	@service yelpService;
	@service store;

	@computed('categoriesService.categories')
	get poppinCategories() {
		return this.categoriesService.categories;
	}

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
	@tracked categories;
	get categoryList() {
		return (this.categories || []).map(c => c.slug);
	}

	@tracked zip;	
	get zipCode() {
		return this.zip ? this.zip.toString().substr(0, 5) : null;
	}

	addlPhotoUrls = eArray(['']);
	@tracked logoUrl;
	@tracked mainPhotoUrl;
	@tracked website;
	@tracked yelpUrl;
	@tracked geo;
	@tracked capacity = '0';
	@tracked crowdSize = '0';
	@tracked hours = _.merge(defHours);
	@tracked visitLength = 45;

	@tracked modalTitle;
	@tracked modalText;
	@tracked showModal = false;

	get locationDTO() {
		const {
			locationId,
			name,
			yelpId,
			capacity,
			crowdSize,
			hours,
			visitLength,
			logoUrl,
			mainPhotoUrl,
			addlPhotoUrls,
			website,
			yelpUrl,
		} = this;
		return  {
			id: locationId,
			yelpId: yelpId,
			name,
			address: {
				line1: this.addressLine1,
				line2: this.addressLine2,
				city: this.city,
				state: this.state,
				zipCode: this.zipCode,
				geo: this.geo
			},
			logoUrl,
			mainPhotoUrl,
			addlPhotoUrls: addlPhotoUrls.filter(p => !!p),
			website,
			yelpUrl,
			categories: this.categoryList,
			capacity: parseInt(capacity, 10),
			crowdSize: parseInt(crowdSize, 10),
			hours,
			visitLength: parseInt(visitLength, 10)
		};
	}

	get canAcceptMatch() {
		return !!this.locationId;
	}

	constructor() {
		super(...arguments);
		this.categoriesService.getCategories()
			.finally(() => this.populateFromPoppin());
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
		this.logoUrl = null,
		this.mainPhotoUrl = null,
		set(this, 'addlPhotoUrls', eArray([''])),
		this.website = null,
		this.yelpUrl = null,
		this.categories = [];
		this.capacity = 0;
		this.hours = _.merge(defHours);
	}

	@action
	cancel() {
		this.clearForm();
		this.args.endEdit();
	}

	@action
	addImg() {
		this.addlPhotoUrls.pushObject("");
	}

	populateFromPoppin(location) {
		const loc = location || this.args.location
		const coordinates = loc.address.geo.coordinates;
		if (loc) {
			this.yelpEntity = loc.yelpDetails;
			this.locationId = loc.id;
			this.yelpId = loc.yelpId;
			this.name = loc.name;
			this.addressLine1 = loc.address.line1;
			this.addressLine2 = loc.address.line2;
			this.city = loc.address.city;
			this.state = loc.address.state;
			this.zip = loc.address.zipCode;
			this.logoUrl = loc.logoUrl;
			this.mainPhotoUrl = loc.mainPhotoUrl;
			set(this, 'addlPhotoUrls', loc.addlPhotoUrls || eArray(['']));
			this.website = loc.website;
			this.yelpUrl = loc.yelpUrl;
			this.geo = {
				type: 'Point',
				coordinates: coordinates.values.length ? coordinates.values : coordinates
			};
			this.categories = (loc.categories || []).map((c) => {
				var matches = this.poppinCategories.filter(pc => pc.slug == c);
				if (matches.length) return matches[0];
			});
			this.capacity = loc.capacity;
			this.hours = loc.hours || _.merge(defHours);
			this.visitLength = loc.visitLength;
		}
	}

	populateFromYelp(loc) {		
		this.yelpEntity = loc;
		this.yelpId = loc.id;
		this.name = loc.name;
		this.addressLine1 = loc.location.address1;
		this.addressLine2 = loc.location.address2;
		this.city = loc.location.city;
		this.state = loc.location.state;
		this.zip = loc.location.zip || this.zip;
		this.yelpUrl = loc.url;
		this.geo = this.geo || {
			type: 'Point',
			coordinates: [loc.coordinates.longitude, loc.coordinates.latitude]
		};
		this.visitLength = this.visitLength > 0 ? this.visitLength : 45;
		
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
					// eslint-disable-next-line no-unused-vars
					.then(loc => loc = location);
			} else {
				this.store.createRecord('location', location);
			}
			this.locationId = location.id;
			this.yelpId = location.yelpId;
			if (typeof this.args.resolveAction == 'function') {
				return this.args.resolveAction(this.locationDTO);
			}
			if (location.yelpId) {
				location.yelpDetails = this.yelpEntity;
				return this.args.redirectToLocation(location);
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
		this.modalText = data.errors.toString();
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
