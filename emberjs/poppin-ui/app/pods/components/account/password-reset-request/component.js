import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';

import { states, actions } from './constants';

export default class 	LoginFormComponent extends StatefulComponent {
	@service accountService;
	_msgs;

	get msgs() {
		return this._msgs || [];
	}

	@tracked email;
	@tracked showMsg;
	@tracked msgType;

	transitions = {
		[states.IDLE]: {
			[actions.SUBMIT]: states.SUBMITTING
		},
		[states.SUBMITTING]: {
			[actions.RESOLVE]: states.IDLE,
			[actions.REJECT]: states.IDLE
		}
	}

	get isLoading() {
		return /ing/i.test(this.machineState);
	}

	constructor() {
		super(...arguments);
		if (this.args.brokenToken) {
			set(this, '_msgs', ['Sorry! It looks like something went wrong with your reset code. Please resubmit the form and try again.']);
		}
		this.initMachine();
	}

	[actions.SUBMIT]() {
		this.accountService.passwordResetRequest({ email: this.email })
			.then(() => this.dispatch(actions.RESOLVE))
			.catch(() => this.dispatch(actions.REJECT));
	}

	[actions.RESOLVE]() {
		this.msgType = 'success';
		set(this, '_msgs', ['Success! Check your email for a message from @wearepoppin.com. Be sure to check your spam filter if you don\'t see it']);
		this.showMsg = true;
	}

	[actions.REJECT]() {
		this.msgType = 'danger';
		set(this, '_msgs', ['Sorry! It looks like something went wrong. Please try again later.']);
		this.showMsg = true;
	}

	@action
	submit() {
		this.dispatch(actions.SUBMIT);
	}

	@action
	hideMsg() {
		this.showMsg = false;
		set(this, '_msgs', []);
	}
}