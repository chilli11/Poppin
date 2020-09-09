import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action } from '@ember/object';
import { inject as service } from '@ember/service';

import { states, actions } from './constants';


export default class VendorFormComponent extends StatefulComponent {
	namespace = 'LocationForm';
	@service vendorsService;
	@service locationsService;
	@service accountService;
	@service store;

	transitions = {
		[states.IDLE]: {
			[actions.SUBMIT_VENDOR]: states.SUBMITTING_VENDOR,
			[actions.ADD_MEMBER]: states.ADDING_MEMBER,
			[actions.GET_LOCATION_ID]: states.GETTING_LOCATION_ID,
			[actions.ADD_LOCATION]: states.ADDING_LOCATION
		},
		[states.SUBMITTING_VENDOR]: {
			[actions.RESOLVE_SUBMIT_VENDOR]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.ADDING_MEMBER]: {
			[actions.RESOLVE_ADD_MEMBER]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.GETTING_LOCATION_ID]: {
			[actions.RESOLVE_GET_LOCATION_ID]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.ADDING_MEMBER]: {
			[actions.RESOLVE_ADD_LOCATION]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
	};

	@tracked showLocationFormModal;
	@tracked showStatusModal;

	@tracked vendorId;
	@tracked parentVendorId;
	@tracked organizationName;
	@tracked organizationContactName;
	@tracked organizationContactEmail;

	_adminIds = [];
	_memberIds = [];
	_subVendorIds = [];
	_locationIds = [];
	
	get adminIds() {
		return this._adminIds;
	}
	get memberIds() {
		return this._memberIds;
	}
	get subVendorIds() {
		return this._subVendorIds;
	}
	get locationIds() {
		return this._locationIds;
	}

	@tracked modalTitle;
	@tracked modalText;
	@tracked showModal = false;

	get isLoading() {
		return /ing/i.test(this.machineState);
	}

	get vendorDTO() {
		const {
			vendorId,
			parentVendorId,
			organizationName,
			organizationContactName,
			organizationContactEmail,
			adminIds,
			memberIds,
			subVendorIds,
			locationIds
		} = this;
		return  {
			id: vendorId,
			parentVendorId,
			organizationName,
			organizationContactName,
			organizationContactEmail,
			adminIds,
			memberIds,
			subVendorIds,
			locationIds
		};
	}

	get canAddMember() {
		return !!this.vendorId;
	}

	constructor() {
		super(...arguments);
		this.populateFromPoppin();
		this.initMachine();
	}

	@action
	clearForm() {
		this.vendorId = null;
		this.parentVendorId = null;
		this.organizationName = null;
		this.organizationContactName = null;
		this.organizationContactEmail = null;
		this._adminIds = [];
		this._memberIds = [];
		this._subVendorIds = [];
		this._locationIds = [];
	}

	populateFromPoppin(vendor) {
		const v = vendor || this.args.vendor;
		if (v) {
			this.vendorId = v.id;
			this.parentVendorId = v.parentVendorId;
			this.organizationName = v.organizationName;
			this.organizationContactName = v.organizationContactName;
			this.organizationContactEmail = v.organizationContactEmail;
			this._adminIds = v.adminIds || [];
			this._memberIds = v.memberIds || [];
			this._subVendorIds = v.subVendorIds || [];
			this._locationIds = v.locationIds || [];
		}
	}

	[actions.SUBMIT_VENDOR]() {
		const method = this.vendorId ? 'updateVendor' : 'createNewVendor';
		return this.vendorsService[method](this.vendorDTO).then((vendor) => {
			if (this.vendorId) {
				this.store.findRecord('vendor', this.vendorId)
					// eslint-disable-next-line no-unused-vars
					.then(v => v = this.vendorDTO);
			} else {
				this.store.createRecord('vendor', vendor);
				this.vendorId = vendor.id;
			}
			if (typeof this.args.resolveAction == 'function') {
				return this.args.resolveAction(vendor);
			}
			
			this.modalText = this.organizationName + " has been added to Poppin!";
			this.modalTitle = "Success!";
			this.showStatusModal = true;
		}).catch(data => this.dispatch(actions.REJECT_ACTION, data));
	}

	[actions.ADD_LOCATION](id) {
		return this._locationIds.indexOf(id) == -1 ? this._locationIds.push(id) : true;
	}

	[actions.GET_LOCATION_ID]() {
		this.showLocationFormModal = true;
	}

	[actions.REJECT_ACTION](data) {
		this.modalText = data.toString();
		this.modalTitle = "Error!";
		this.showModal = true;
		return console.error(data);
	}


	@action
	submit() {
		return this.dispatch(actions.SUBMIT_VENDOR, null, true);
	}

	@action
	addLocation(loc) {
		return this.dispatch(actions.ADD_LOCATION, loc.id);
	}

	@action
	addMember(email) {
		this.dispatch(actions.ADD_MEMBER, email);
	}
}
