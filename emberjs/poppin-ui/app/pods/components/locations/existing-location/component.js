import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action, computed } from '@ember/object';
import { inject as service } from '@ember/service';
import { states, actions } from './constants';
import { tracked } from '@glimmer/tracking';

export default class LocationFormComponent extends StatefulComponent {
	@service store;
	@service locationsService;
	@service categoriesService;
	@service accountService;
	
	@tracked modalTitle;
	@tracked modalText;
	@tracked showModal = false;

	@computed('accountService.authInfo')
	get authInfo() {
		return this.accountService.authInfo;
	}
	@computed('accountService.accountInfo')
	get accountInfo() {
		return this.accountService.accountInfo;
	}
	@computed('accountService.profile')
	get profile() {
		return this.accountService.profile;
	}

	// eslint-disable-next-line ember/require-computed-property-dependencies
	@computed('categoriesService.categories')
	get fullCategories() {
		const cats = this.categoriesService.categories;
		return cats.filter(c => (this.args.location.categories || []).indexOf(c.slug) > -1);
	}

	get authorized() {
		return this.authInfo && this.authInfo.authorized;
	}
	get isAdmin() {
		const accAdmin = this.accountInfo && this.accountInfo.role == 'Admin';
		const profAdmin = this.profile && this.profile.role == 'Admin';
		return accAdmin || profAdmin;
	}
	get isVendor() {
		const vendorId = this.args.location.vendorId;
		return vendorId && (this.profile.vendorIds || []).indexOf(vendorId) !== -1;
	}

	// eslint-disable-next-line ember/require-computed-property-dependencies
	@computed('accountService.profile.favorites')
	get isFavorite() {
		const locId = this.args.location.id;
		return this.profile ? (this.profile.favorites || []).indexOf(locId) !== -1 : false;
	}

	transitions = {
		[states.IDLE]: {
			[actions.START_EDIT]: states.EDIT_LOCATION,
			[actions.FAV_ACTION]: states.LOADING,
			[actions.UPDATE_CROWD_SIZE]: states.LOADING
		},
		[states.EDIT_LOCATION]: {
			[actions.END_EDIT]: states.IDLE
		},
		[states.LOADING]: {
			[actions.END_LOADING]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		}
	};

	constructor() {
		super(...arguments);
		this.initMachine();
	}

	[actions.END_EDIT](data) {
		this.args.refresh(data);
	}

	[actions.FAV_ACTION]() {
		const method = this.isFavorite ? 'removeFavorite' : 'addFavorite';
		return this.accountService[method](this.args.location.id)
			.then(() => {
				console.log(this.isFavorite);
				return this.dispatch(actions.END_LOADING);
			}).catch(({ errors }) => this.dispatch(actions.REJECT_ACTION, errors));
	}

	[actions.UPDATE_CROWD_SIZE](data) {
		const method = data > 0 ? 'incrementCrowd' : 'decrementCrowd';
		return this.locationsService[method](this.args.location.id)
			.then((location) => {
				this.store.findRecord('location', location.id)
					.then((loc) => loc.crowdSize = location.crowdSize);
				location.yelpDetails = this.args.location.yelpDetails;
				this.args.refresh(location);
				return this.dispatch(actions.END_LOADING);
			}).catch(({ errors }) => this.dispatch(actions.REJECT_ACTION, errors));
	}

	[actions.REJECT_ACTION](data) {
		this.modalText = data.toString();
		this.modalTitle = "Error!";
		this.showModal = true;
		return console.error(data);
	}

	@action
	startEdit() {
		return this.dispatch(actions.START_EDIT);
	}

	@action
	endEdit(data) {
		return this.dispatch(actions.END_EDIT, data);
	}

	@action
	increment() {
		return this.dispatch(actions.UPDATE_CROWD_SIZE, 1);
	}

	@action
	decrement() {
		return this.dispatch(actions.UPDATE_CROWD_SIZE, -1);
	}

	@action
	favAction() {
		return this.dispatch(actions.FAV_ACTION, this.args.location.id);
	}

}
