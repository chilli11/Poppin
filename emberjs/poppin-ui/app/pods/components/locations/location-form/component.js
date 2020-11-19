import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set, computed } from '@ember/object';
import { Menu } from 'poppin-ui/classes/location-entities';
import { inject as service } from '@ember/service';
import _ from 'lodash';
import StatesWithCodes from 'poppin-ui/utils/states-with-codes';

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
	statesWithCodes = StatesWithCodes;

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

  addlPhotoUrls = [];
  menus = []
	@tracked addlPhotoUrl;
	@tracked logoUrl;
	@tracked mainPhotoUrl;
	@tracked website;
	@tracked menuName;
	@tracked menuUrl;
	@tracked yelpUrl;
	@tracked geo;
	@tracked capacity = '0';
	@tracked crowdSize = '0';
	@tracked hours = _.merge(defHours);
	@tracked visitLength = 45;

	@computed('addlPhotoUrls')
	get photoUrls() {
		return this.addlPhotoUrls;
	}
	@computed('menus')
	get oldMenus() {
		return this.menus;
	}

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
			addlPhotoUrl,
			website,
			menus,
			menuName,
			menuUrl,
			yelpUrl,
		} = this;
		var output = {
			id: locationId,
			yelpId: yelpId,
			name,
			address: {
				line1: this.addressLine1,
				line2: this.addressLine2,
				city: this.city,
				state: this.state ? this.state.value : null,
				zipCode: this.zipCode,
				geo: this.geo
			},
			logoUrl,
			mainPhotoUrl,
			addlPhotoUrls: addlPhotoUrls.concat(addlPhotoUrl).filter(p => !!p),
			website,
			menus,
			yelpUrl,
			categories: this.categoryList,
			capacity: parseInt(capacity, 10),
			crowdSize: parseInt(crowdSize, 10),
			hours,
			visitLength: parseInt(visitLength, 10)
		};
		if (menuUrl) {
			const newMenu = new Menu(menuUrl, menuName);
			output.menus = menus.concat(newMenu).filter(m => !!m.url)
		}
		return output;
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
		this.zip = null;
		this.logoUrl = null;
		this.mainPhotoUrl = null;
		set(this, 'addlPhotoUrls', []);
		this.addlPhotoUrl = null;
		this.website = null;
		set(this, 'menus', []);
		this.menuUrl = null;
		this.menuName = null;
		this.yelpUrl = null;
		this.categories = [];
		this.capacity = 0;
		this.hours = _.merge(defHours);
	}

	@action
	cancel() {
		this.args.resolveAction();
	}

	@action
	addImg() {
		this.addlPhotoUrls.pushObject("");
	}

	@action
	removePic(url) {
		this.addlPhotoUrls.removeObject(url);
	}

	@action
	removeMenu(menu) {
		this.menus.removeObject(menu);
	}

	populateFromPoppin(location) {
		const loc = location || this.args.location
		if (loc) {
			const coordinates = loc.address.geo.coordinates;
			this.yelpEntity = loc.yelpDetails;
			this.locationId = loc.id;
			this.yelpId = loc.yelpId;
			this.name = loc.name;
			this.addressLine1 = loc.address.line1;
			this.addressLine2 = loc.address.line2;
			this.city = loc.address.city;
			this.state = this.statesWithCodes.filter(s => s.value == loc.address.state)[0];
			this.zip = loc.address.zipCode;
			this.logoUrl = loc.logoUrl;
			this.mainPhotoUrl = loc.mainPhotoUrl;
			set(this, 'addlPhotoUrls', loc.addlPhotoUrls || []);
			this.addlPhotoUrl = null;
			set(this, 'menus', (loc.menus || []).map(m => new Menu(m.url, m.name)));
			this.menuName = null;
			this.menuUrl = null;
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
		this.state = this.statesWithCodes.filter(s => s.value == loc.location.state)[0];
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
			}
			return this.args.redirectToLocation(location);
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
