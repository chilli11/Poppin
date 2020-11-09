import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
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
			[actions.REMOVE_MEMBER]: states.REMOVING_MEMBER,
			[actions.GET_LOCATION_ID]: states.GETTING_LOCATION_ID,
			[actions.ADD_LOCATION]: states.ADDING_LOCATION,
			[actions.REMOVE_LOCATION]: states.REMOVING_LOCATION
		},
		[states.SUBMITTING_VENDOR]: {
			[actions.RESOLVE_SUBMIT_VENDOR]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.ADDING_MEMBER]: {
			[actions.RESOLVE_MEMBER]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.REMOVING_MEMBER]: {
			[actions.RESOLVE_MEMBER]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.GETTING_LOCATION_ID]: {
			[actions.RESOLVE_GET_LOCATION_ID]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.ADDING_LOCATION]: {
			[actions.RESOLVE_LOCATION]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
		[states.REMOVING_LOCATION]: {
			[actions.RESOLVE_LOCATION]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		},
	};

	@tracked showLocationSearchModal;
	@tracked showMemberAddModal;
	@tracked showStatusModal;

	@tracked showMemberMsg;
	@tracked memberMsgType = "success";
	memberMsgs = [];

	@tracked showMsg = false;
	@tracked msgType = "success";
	msgs = [];


	@tracked vendorId;
	@tracked parentVendorId;
	@tracked organizationName;
	@tracked organizationContactName;
	@tracked organizationContactEmail;

	@tracked newMemberEmail;
	@tracked newMemberRole;

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
		return this.vendorsService.addLocation({
			vendorId: this.vendorId,
			locationId: id
		}).then((data) => this.dispatch(actions.RESOLVE_LOCATION, data))
		.catch((data) => this.dispatch(actions.REJECT_ACTION, { msgs: data.errors }));
	}

	[actions.ADD_MEMBER]() {
		return this.vendorsService.addMember({
			vendorId: this.vendorId,
			email: this.newMemberEmail,
			role: this.newMemberRole,
			userId: null,
		}).then((vendor) => this.dispatch(actions.RESOLVE_MEMBER, vendor))
			.catch((data) => this.dispatch(actions.REJECT_ACTION, { msgs: data.errors, context: 'member' }));
	}

	[actions.GET_LOCATION_ID]() {
		this.showLocationFormModal = true;
	}

	[actions.RESOLVE_MEMBER]({ admins, members, vendor }) {
		set(this, 'memberMsgs', ['Success!']);
		this.memberMsgType = 'success';
		this.showMemberMsg = true;

		const vendor = vendor;
		vendor.admins = admins;
		vendor.members = members;
		this.args.refresh(vendor);
	}

	[actions.RESOLVE_LOCATION](data) {
		set(this, 'msgs',  ['Location added successfully']);
		this.msgType = 'success';
		this.showMsg = true;

		const vendor = this.args.vendor;
		vendor.locations = data.locations;
		vendor.locationIds = data.vendor.locationIds;
		this.args.refresh(vendor);
	}

	[actions.REJECT_ACTION]({ msgs, context }) {
		if (context && context == 'member') {
			set(this, 'memberMsgs', msgs || []);
			this.memberMsgType = 'danger';
			this.showMemberMsg = true;
		} else {
			set(this, 'msgs',  msgs || []);
			this.msgType = 'danger';
			this.showMsg = true;
		}
	}


	@action
	submit() {
		return this.dispatch(actions.SUBMIT_VENDOR, null, true);
	}

	@action
	addLocation(loc) {
		this.closeLocationSearchModal();
		return this.dispatch(actions.ADD_LOCATION, loc.id);
	}

	@action
	addMember() {
		this.dispatch(actions.ADD_MEMBER);
	}

	@action
	startAddMember() {
		this.showMemberAddModal = true;
		this.newMemberRole = 'Member';
	}

	@action
	startAddLocation() {
		this.showLocationSearchModal = true;
	}

	@action
	closeStatusModal() {
		this.showStatusModal = false;
	}

	@action
	closeMemberAddModal() {
		this.showMemberAddModal = false;
	}

	@action
	closeLocationSearchModal() {
		this.showLocationSearchModal = false;
	}

	@action
	hideMsg(context) {
		this.showMsg = false;
		set(this, context == 'member' ? 'memberMsgs' : 'msgs', []);
	}
}
