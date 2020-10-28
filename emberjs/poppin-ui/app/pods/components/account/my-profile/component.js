import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action, set, computed } from '@ember/object';
import { inject as service } from '@ember/service';
import { tracked } from '@glimmer/tracking';
import { states, actions } from './constants';

export default class Component extends StatefulComponent {
	@service accountService;
	@service yelpService;

	@tracked isEdit = false;
	@tracked showStatusMsg;
	@tracked statusType;
	statusMsgs = [];
	
	ageRanges = [
		{ key: "", value: "" },
		{ key: "u18", value: "Under 18" },
		{ key: "u26", value: "18-25" }, 
		{ key: "u36", value: "26-35" },
		{ key: "u46", value: "36-45" },
		{ key: "u56", value: "46-55" },
		{ key: "o55", value: "Over 55" }
	];
	genders = [
		{ key: "", value: "" },
		{ key: "M", value: "Male" },
		{ key: "F", value: "Female" },
		{ key: "O", value: "Other" },
		{ key: "D", value: "Prefer Not to Say" }
	];
	yelpCategories;
	yc;

	get ageRange() {
		var filter = this.ageRanges.filter(a => a.key == this.args.profile.ageRange);
		return filter && filter.length ? filter[0] : null;
	}
	get gender() {
		var filter = this.genders.filter(g => g.key == this.args.profile.gender);
		return filter && filter.length ? filter[0] : null;
	}

	@computed('args.profile.categories', 'yelpCategories', 'yc')
	get categories() {
		return (this.yelpCategories || [])
			.filter(yc => (this.args.profile.categories || []).indexOf(yc.alias) > -1);
	}

	transitions = {
		[states.IDLE]: {
			[actions.START_EDIT]: states.EDIT_PROFILE
		},
		[states.EDIT_PROFILE]: {
			[actions.END_EDIT]: states.IDLE
		},
		[states.LOADING]: {
			[actions.END_LOADING]: states.IDLE
		}
	};

	constructor() {
		super(...arguments);
		this.initMachine();
		this.yelpService.getYelpCategories()
			.then(data => set(this, 'yelpCategories', data));
	}

	[actions.START_EDIT]() {
		this.isEdit = true;
	}

	[actions.END_EDIT](data) {
		this.isEdit = false;
		if (data) this.args.refresh(data);
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
	hideMsg() {
		this.showModalMsg = false;
		this.showStatusMsg = false;
	}
}
