import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';

import { states, actions } from './constants';

export default class 	LoginFormComponent extends StatefulComponent {
	namespace = 'LoginForm';
	@service accountService;
	@service store;
	@service router;

	transitions = {
		[states.IDLE]: {
			[actions.SUBMIT]: states.SUBMITTING
		},
		[states.ERROR]: {
			[actions.SUBMIT]: states.SUBMITTING
		},
		[states.SUBMITTING]: {
			[actions.REJECT]: states.ERROR,
			[actions.RESOLVE]: states.SUCCESS
		}
	};

	@tracked email;
	@tracked password;

	@tracked showMsg = false;
	@tracked msgType = "success";
	msgs = [];

	constructor() {
		super(...arguments);
		this.initMachine();
	}

	[actions.SUBMIT]() {
		this.showMsg = false;
		const { email, password } = this;
		this.showMsg = false;
		this.accountService.login({ email, password })
			.then((response) => {
				if (response.errors && response.errors.length) throw response;
				return this.accountService.myProfile()
					.then(() => this.dispatch(actions.RESOLVE, ['Login success!']));
			}).catch((response) => this.dispatch(actions.REJECT, response.errors));
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
		return this.args.refresh();
	}

	@action
	clearForm() {
		this.email = '';
		this.password = '';
	}

	@action
	submit() {
		return this.dispatch(actions.SUBMIT);
	}

	@action
	hideMsg() {
		this.showMsg = false;
		set(this, 'msgs', []);
	}
}