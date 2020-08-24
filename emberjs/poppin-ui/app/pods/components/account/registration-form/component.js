import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';
import _ from 'lodash';

import { states, actions } from './constants';

const invalidPasswordErrorMsg = 'Password must be 8-24 characters and contain at least 1 of each of the following: '
															+ 'lowercase and uppercase letter, number, and special character';

export default class 	RegistrationFormComponent extends StatefulComponent {
	namespace = 'RegistrationForm';
	@service accountService;
	@service store;

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
	};

	@tracked email;
	@tracked password;
	@tracked password2;

	@tracked showMsg = false;
	@tracked msgType = "success";
	msgs = [];

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
		this.showMsg = false;
		const msgs = [];
		if (!this.isValidPassword) msgs.push(invalidPasswordErrorMsg);
		if (this.password !== this.password2) msgs.push('Passwords don\'t match');
		if (msgs.length) return this.dispatch(actions.REJECT, msgs);

		set(this, 'msgs', msgs);
		return this.dispatch(actions.SUBMIT);
	}

	[actions.SUBMIT]() {
		this.showMsg = false;
		const { email, password, password2 } = this;
		this.showMsg = false;
		this.accountService.registerAccount({ email, password, password2 })
			.then((response) => {
				if (response.errors && response.errors.length) throw response;
				return this.dispatch(actions.RESOLVE, ['Registrtaion success!']);
			}).catch((response) => {
				console.log(response);
				return this.dispatch(actions.REJECT, response.errors);
			});
	}

	[actions.REJECT](msgs) {
		set(this, 'msgs',  msgs || []);
		this.msgType = 'danger';
		this.showMsg = true;
	}

	[actions.RESOLVE](msgs) {
		set(this, 'msgs', msgs || []);
		this.msgType = 'success';
		this.showMsg = true;
	}

	@action
	clearForm() {
		this.email = '';
		this.password = '';
		this.password2 = '';
	}

	@action
	submit() {
		return this.dispatch(actions.VALIDATE_PASSWORD);
	}

	@action
	hideMsg() {
		this.showMsg = false;
		set(this, 'msgs', []);
	}
}