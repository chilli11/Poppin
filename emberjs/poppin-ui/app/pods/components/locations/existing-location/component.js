import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action, computed } from '@ember/object';
import { A } from '@ember/array';
import { inject as service } from '@ember/service';
import { states, actions } from './constants';
import { tracked } from '@glimmer/tracking';

const priceOptions = [
	{ value: 1, name: '$' },
	{ value: 2, name: '$$' },
	{ value: 3, name: '$$$' },
	{ value: 4, name: '$$$$' }
];

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

	@tracked showAlert;
	@tracked alertType;
	@tracked alertMsg;

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

	get price() {
		return priceOptions[(this.args.location.price || 1) - 1];
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
			[actions.HIDE_LOGIN_MODAL]: states.QR_MODAL
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
				return this.dispatch(actions.RESOLVE_ACTION);
			}).catch(data => this.dispatch(actions.REJECT_ACTION, data.errors));
	}

	[actions.UPDATE_CROWD_SIZE](data) {
		const method = data > 0 ? 'incrementCrowd' : 'decrementCrowd';
		return this.locationsService[method](this.args.location.id)
			.then((location) => {
				this.store.findRecord('location', location.id)
					.then((loc) => loc.crowdSize = location.crowdSize);
				// location.yelpDetails = this.args.location.yelpDetails;
				this.args.refresh(location);
				return this.dispatch(actions.END_LOADING);
			}).catch(() => this.dispatch(actions.REJECT_ACTION, { errors: data < 0 ? 'There were no eligible checkins to remove.' : 'Something went wrong...'}));
	}

	[actions.CHECKIN]() {
		return this.locationsService.checkin(this.args.location.id)
			.then((location) => {
				this.store.findRecord('location', location.id)
					.then((loc) => loc.crowdSize = location.crowdSize);
				this.args.refresh(location);
				return this.dispatch(actions.RESOLVE_ACTION, "You've checked in successfully");
			});
	}

	[actions.RESOLVE_ACTION](msg) {
			this.showQrModal = false;
			this.showLoginModal = false;

			this.showAlert = true;
			this.alertMsg = msg || 'Success!';
			this.alertType = 'success';
	}

	[actions.REJECT_ACTION](data) {
		this.showQrModal = false;
		this.showLoginModal = false;

		this.alertMsg = data.errors.toString() || "Error!";
		this.alertType = 'danger';
		this.showAlert = true;
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
	hideAlert() {
		this.showAlert = false;
		this.alertMsg = null;
		this.alertType = null;
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
