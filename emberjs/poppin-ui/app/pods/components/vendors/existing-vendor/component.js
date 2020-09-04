import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action } from '@ember/object';
import { inject as service } from '@ember/service';
import { states, actions } from './constants';

export default class VendorFormComponent extends StatefulComponent {
	@service store;

	transitions = {
		[states.IDLE]: {
			[actions.START_EDIT]: states.EDIT_LOCATION,
		},
		[states.EDIT_LOCATION]: {
			[actions.END_EDIT]: states.IDLE
		},
		[states.LOADING]: {
			[actions.END_LOADING]: states.IDLE
		}
	};

	constructor() {
		super(...arguments);
		this.initMachine();
	}

	[actions.END_EDIT](data) {
		this.args.refresh(data);
	}

	@action
	startEdit() {
		return this.dispatch(actions.START_EDIT);
	}

	@action
	endEdit(data) {
		return this.dispatch(actions.END_EDIT, data);
	}
}
