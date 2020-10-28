import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';

import { states, actions } from './constants';


export default class VendorFormComponent extends StatefulComponent {
	namespace = 'LocationForm';
	@service accountService;

	transitions = {
		[states.IDLE]: {
			[actions.SUBMIT_PROFILE]: states.SUBMITTING_PROFILE
		},
		[states.SUBMITTING_PROFILE]: {
			[actions.RESOLVE_SUBMIT_PROFILE]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		}
	};

	@tracked showMsg = false;
	@tracked msgType = "success";
	msgs = [];
	
	categories = [];

	@tracked firstName;
	@tracked lastName;
	@tracked profilePhoto;
	@tracked ageRange;
	@tracked gender;
	get categoryList() {
		return this.categories.map(c => c.alias);
	}

	get isLoading() {
		return /ing/i.test(this.machineState);
	}

	get profileDTO() {
		const profile = this.args.profile;
		const {
			firstName,
			lastName,
			profilePhoto,
			ageRange,
			gender,
			categoryList
		} = this;
		return  {
			userId: profile.userId,
			username: profile.username,
			firstName,
			lastName,
			email: profile.email,
			profilePhoto,
			ageRange: ageRange ? ageRange.key : null,
			gender: gender ? gender.key : null,
			categories: categoryList
		};
	}

	constructor() {
		super(...arguments);
		this.populateFromPoppin();
		this.initMachine();
	}

	@action
	clearForm() {
		this.profilePhoto = null;
		this.ageRange = null;
		this.gender = null;
		this.categories = [];
	}

	populateFromPoppin(profile) {
		const p = profile || this.args.profile;
		if (p) {
			this.firstName = p.firstName;
			this.lastName = p.lastName;
			this.profilePhoto = p.profilePhoto;
			const arMatch = this.args.ageRanges.filter(ar => ar.key == p.ageRange);
			const gMatch = this.args.genders.filter(g => g.key == p.gender);
			this.ageRange = arMatch && arMatch.length ? arMatch[0] : null;
			this.gender = gMatch && gMatch.length ? gMatch[0] : null;
			this.categories = this.args.categories;
		}
	}

	[actions.SUBMIT_PROFILE]() {
		return this.accountService.updateProfile(this.profileDTO).then((profile) => {
			if (typeof this.args.resolveAction == 'function') {
				return this.args.resolveAction(profile);
			}	

			this.modalText = "Your profile has been updated!";
			this.modalTitle = "Success!";
			this.showStatusModal = true;
			return this.dispatch(actions.RESOLVE_SUBMIT_PROFILE);
		}).catch(data => this.dispatch(actions.REJECT_ACTION, data));
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
		return this.dispatch(actions.SUBMIT_PROFILE, null, true);
	}

	@action
	hideMsg() {
		this.showMsg = false;
		set(this, 'msgs', []);
	}
}
