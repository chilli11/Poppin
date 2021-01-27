import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';
import { tracked } from '@glimmer/tracking';
import { states, actions } from './constants';

export default class VendorFormComponent extends StatefulComponent {
	@service store;
	@service vendorsService;
	@service accountService;

	@tracked showLocationModal;
	@tracked showRemoveLocationModal;
	@tracked modalLoc;
	@tracked showMemberModal;
	@tracked modalMember;
	@tracked isEdit = false;
	get memberIsAdmin() {
		if (this.args.vendor && this.args.vendor.adminIds && this.modalMember)
			return this.args.vendor.adminIds.indexOf(this.modalMember.userId) > -1;
		return false;
	}

	get profile() {
		return this.accountService.profile;
	}
	get isSiteAdmin() {
		return this.profile.role == 'Admin';
	}
	get isVendorAdmin() {
		return this.args.vendor && this.args.vendor.adminIds && this.args.vendor.adminIds.indexOf(this.profile.userId) > -1;
	}

	@tracked showStatusMsg;
	@tracked showModalMsg;
	@tracked statusType;
	statusMsgs = [];

	transitions = {
		[states.IDLE]: {
			[actions.START_EDIT]: states.EDIT_VENDOR,
			[actions.VIEW_LOCATION]: states.LOCATION_MODAL,
			[actions.VIEW_MEMBER]: states.MEMBER_MODAL
		},
		[states.EDIT_VENDOR]: {
			[actions.END_EDIT]: states.IDLE,
			[actions.VIEW_LOCATION]: states.LOCATION_MODAL
		},
		[states.LOADING]: {
			[actions.END_LOADING]: states.IDLE,
			[actions.CLOSE_LOCATION]: states.IDLE,
			[actions.CLOSE_MEMBER]: states.IDLE,
			[actions.CLOSE_REMOVE_LOCATION]: states.IDLE
		},
		[states.LOCATION_MODAL]: {
			[actions.REMOVE_LOCATION]: states.REMOVE_LOCATION_MODAL,
			[actions.CLOSE_LOCATION]: states.IDLE
		},
		[states.REMOVE_LOCATION_MODAL]: {
			[actions.CONFIRM_REMOVE_LOCATION]: states.LOADING,
			[actions.CLOSE_REMOVE_LOCATION]: states.IDLE
		},
		[states.MEMBER_MODAL]: {
			[actions.ADD_MEMBER]: states.LOADING,
			[actions.REMOVE_MEMBER]: states.LOADING,
			[actions.CLOSE_MEMBER]: states.IDLE
		}
	};

	constructor() {
		super(...arguments);
		this.initMachine();
	}

	[actions.START_EDIT]() {
		this.isEdit = true;
	}

	[actions.END_EDIT](data) {
		this.isEdit = false;
		if (data) this.args.refresh(data);
	}

	[actions.VIEW_LOCATION](loc) {
		this.modalLoc = loc;
		this.showLocationModal = true;
	}

	[actions.CLOSE_LOCATION]() {
		this.modalLoc = null;
		this.showLocationModal = false;
	}

	[actions.VIEW_MEMBER](member) {
		this.modalMember = member;
		this.showMemberModal = true;
	}

	[actions.CLOSE_MEMBER]() {
		this.modalMember = null;
		this.showMemberModal = false;
	}

	[actions.REMOVE_LOCATION](locationId) {
		this.hideMsg();
		this.showLocationModal = false;
		this.showRemoveLocationModal = true;
	}

	[actions.CLOSE_REMOVE_LOCATION]() {
		this.hideMsg();
		this.modalLoc = null;
		this.showRemoveLocationModal = false;
	}

	[actions.CONFIRM_REMOVE_LOCATION](locationId) {
		this.hideMsg();
		return this.vendorsService.removeLocation({
			vendorId: this.args.vendor.id,
			locationId
		}).then((data) => {
			this.closeRemoveLocationModal();
			set(this, 'statusMsgs', ['Location Successfully Removed']);
			this.statusType = 'success';
			this.showStatusMsg = true;

			const vendor = this.args.vendor;
			vendor.locations = data.locations;
			vendor.locationIds = data.vendor.locationIds;
			this.args.refresh(vendor);
		}).catch(({ errors }) => this.showErrors(errors, true));
	}

	[actions.ADD_MEMBER](member) {
		return this.vendorsService.addMember({
			vendorId: this.args.vendor.id,
			email: member.email,
			role: member.role,
			userId: null,
		}).then((data) => this.memberSuccess(data))
			.catch(({ errors }) => this.showErrors(errors, true));
	}

	[actions.REMOVE_MEMBER](member) {
		return this.vendorsService.removeMember({
			vendorId: this.args.vendor.id,
			email: member.email,
			role: member.role,
			userId: member.userId,
		}).then((data) => this.memberSuccess(data))
			.catch(({ errors }) => this.showErrors(errors, true));
	}

	memberSuccess({ admins, members, adminIds, memberIds }) {
		this.dispatch(actions.CLOSE_MEMBER);
		set(this, 'statusMsgs', ['Success!']);
		this.statusType = 'success';
		this.showStatusMsg = true;

		const vendor = this.args.vendor;
		vendor.admins = admins;
		vendor.members = members;
		vendor.adminIds = adminIds;
		vendor.memberIds = memberIds;
		this.args.refresh(vendor);
	}

	showErrors(errors, isModal) {
		set(this, 'statusMsgs', errors || ['Something went wrong...']);
		this.statusType = 'danger';
		if (isModal)
			this.showModalMsg = true;
		else	
			this.showStatusMsg = true;
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
	openLocationModal(loc) {
		return this.dispatch(actions.VIEW_LOCATION, loc);
	}

	@action
	closeLocationModal() {
		this.dispatch(actions.CLOSE_LOCATION);
	}

	@action
	removeLocation(loc) {
		return this.dispatch(actions.REMOVE_LOCATION, loc.id);
	}

	@action
	confirmRemoveLocation(loc) {
		return this.dispatch(actions.CONFIRM_REMOVE_LOCATION, loc.id);
	}
	
	@action
	closeRemoveLocationModal() {
		return this.dispatch(actions.CLOSE_REMOVE_LOCATION);
	}

	@action
	openMemberModal(member) {
		return this.dispatch(actions.VIEW_MEMBER, member);
	}

	@action
	closeMemberModal() {
		this.dispatch(actions.CLOSE_MEMBER);
	}

	@action
	changeRole(member) {
		if (this.memberIsAdmin) {
			return this.dispatch(actions.REMOVE_MEMBER, member);
		}
		return this.dispatch(actions.ADD_MEMBER, member);
	}

	@action
	removeMember(member) {
		return this.dispatch(actions.REMOVE_MEMBER, member);
	}

	@action
	hideMsg() {
		this.showModalMsg = false;
		this.showStatusMsg = false;
	}
}
