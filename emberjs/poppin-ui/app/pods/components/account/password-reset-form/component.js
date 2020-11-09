import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';

import { states, actions } from './constants';

export default class 	LoginFormComponent extends StatefulComponent {
	@service accountService;

	transitions = {
		[states.IDLE]: {
			[actions.VALIDATE_PASSWORD]: states.VALIDATING
		},
		[states.ERROR]: {
			[actions.VALIDATE_PASSWORD]: states.VALIDATING
		},
		[states.VALIDATING]: {
			[actions.REJECT]: states.ERROR,
			[actions.SUBMIT]: states.SUBMITTING
		},
		[states.SUBMITTING]: {
			[actions.REJECT]: states.ERROR,
			[actions.RESOLVE]: states.SUCCESS
		}
	}

	_msgs;
	get msgs() {
		return this._msgs || [];
	}

	@tracked password;
	@tracked password2;
	@tracked showMsg;
	@tracked msgType;
	get isLoading() {
		return /ing/i.test(this.machineState);
	}

	get isValidPassword() {
		const hasUpper = /[A-Z]+/.test(this.password);
		const hasLower = /[a-z]+/.test(this.password);
		const hasNumber = /[0-9]+/.test(this.password);
		const hasSpecial = /[_!@#$%^&*]+/.test(this.password);
		const isOnlyAllowed = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[_!@#$%^&*])(?=.{8,24})/i.test(this.password);
		return hasUpper && hasLower && hasNumber && hasSpecial && isOnlyAllowed;
	}

	constructor() {
		super(...arguments);
		this.initMachine();
	}

	[actions.VALIDATE_PASSWORD]() {
		this.hideMsg();
		const msgs =[];
		if (!this.isValidPassword) msgs.push(invalidPasswordErrorMsg);
		if (this.password !== this.password2) msgs.push('Passwords don\'t match');
		if (msgs.length) return this.dispatch(actions.REJECT, msgs);

		set(this, '_msgs', msgs);
		return this.dispatch(actions.SUBMIT);
	}

	[actions.SUBMIT]() {
		const { password, password2 } = this;
		this.hideMsg();
		this.accountService.resetPassword(this.args.userId, { token: this.args.token, password, password2 })
			.then((response) => {
				if (response.errors && response.errors.length) throw response;
				return this.dispatch(actions.RESOLVE);
			}).catch(() => this.dispatch(actions.REJECT));
	}

	[actions.RESOLVE]() {
		this.msgType = 'success';
		set(this, '_msgs', ['Success! Your password has been updated.']);
		this.showMsg = true;
	}

	[actions.REJECT](msgs) {
		this.msgType = 'danger';
		set(this, '_msgs', msgs || ['Sorry! It looks like something went wrong. Please try again later.']);
		this.showMsg = true;
	}

	@action
	clearForm() {
		this.password = null;
		this.password2 = null;
	}

	@action
	submit() {
		return this.dispatch(actions.VALIDATE_PASSWORD);
	}

	@action
	hideMsg() {
		this.showMsg = false;
		set(this, '_msgs', []);
	}
}