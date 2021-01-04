import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action, computed } from '@ember/object';
import { A } from '@ember/array';
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

	@tracked showLoginModal;
	@tracked showQrModal;

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
		return A(cats.filter(c => (this.args.location.categories || []).indexOf(c.slug) > -1));
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
		return vendorId && this.profile && (this.profile.vendorIds || []).indexOf(vendorId) !== -1;
	}

	// eslint-disable-next-line ember/require-computed-property-dependencies
	@computed('accountService.profile.favorites')
	@tracked isFavorite = (() => {
		const locId = this.args.location.id;
		return this.profile ? (this.profile.favorites || []).indexOf(locId) !== -1 : false;
	})();

	transitions = {
		[states.IDLE]: {
			[actions.START_EDIT]: states.EDIT_LOCATION,
			[actions.FAV_ACTION]: states.LOADING,
			[actions.UPDATE_CROWD_SIZE]: states.LOADING,
			[actions.SHOW_QR_MODAL]: states.QR_MODAL
		},
		[states.EDIT_LOCATION]: {
			[actions.END_EDIT]: states.IDLE
		},
		[states.QR_MODAL]: {
			[actions.SHOW_LOGIN_MODAL]: states.LOGIN_MODAL,
			[actions.HIDE_QR_MODAL]: states.IDLE,
			[actions.CHECKIN]: states.LOADING
		},
		[states.LOGIN_MODAL]: {
			[actions.CHECKIN]: states.LOADING,
			[actions.HIDE_LOGIN_MODAL]: states.IDLE
		},
		[states.LOADING]: {
			[actions.END_LOADING]: states.IDLE,
			[actions.RESOLVE_ACTION]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		}
	};

	constructor() {
		super(...arguments);
		this.initMachine();
	}

	init() {
		if (this.args.qr) {
			this.dispatch(actions.SHOW_QR_MODAL);
		}
	}

	[actions.END_EDIT](data) {
		this.args.refresh(data);
	}

	[actions.FAV_ACTION]() {
		const method = this.isFavorite ? 'removeFavorite' : 'addFavorite';
		return this.accountService[method](this.args.location.id)
			.then(() => {
				this.isFavorite = !this.isFavorite;
				return this.dispatch(actions.END_LOADING);
			}).catch(data => this.dispatch(actions.REJECT_ACTION, data.errors));
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
			}).catch(() => this.dispatch(actions.REJECT_ACTION, { errors: data < 0 ? 'There were no eligible checkins to remove.' : 'Something went wrong...'}));
	}

	[actions.CHECKIN]() {
		return this.locationsService.checkin(this.args.location.id)
			.then(() => this.dispatch(actions.RESOLVE_ACTION));
	}

	[actions.RESOLVE_ACTION]() {
		this.modalMsg = 'Success!'
		setTimeout(() => {
			this.showQrModal = false;
			this.showLoginModal = false;
			this.modalMsg = null;
		}, 2000);
	}

	[actions.REJECT_ACTION](data) {
		this.modalText = data.errors.toString();
		this.modalTitle = "Error!";
		this.showModal = true;
		return console.error(data);
	}

	[actions.SHOW_QR_MODAL]() {
		this.showQrModal = true;
	}

	[actions.HIDE_QR_MODAL]() {
		this.showQrModal = false;
	}

	[actions.SHOW_LOGIN_MODAL]() {
		this.showLoginModal = true;
	}

	[actions.HIDE_LOGIN_MODAL]() {
		this.showLoginModal = false;
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

	@action
	startLoginModal() {
		return this.dispatch(actions.SHOW_LOGIN_MODAL);
	}

	@action
	startQrModal() {
		return this.dispatch(actions.SHOW_QR_MODAL);
	}

	@action
	endLoginModal() {
		return this.dispatch(actions.HIDE_LOGIN_MODAL);
	}

	@action
	endQrModal() {
		return this.dispatch(actions.HIDE_QR_MODAL);
	}

	@action
	checkin() {
		return this.dispatch(actions.CHECKIN);
	}
}
