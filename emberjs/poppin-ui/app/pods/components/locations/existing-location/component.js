import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action } from '@ember/object';
import { inject as service } from '@ember/service';
import { states, actions } from './constants';
import { tracked } from '@glimmer/tracking';

export default class LocationFormComponent extends StatefulComponent {
	@service store;
	@service locationsService;
	
	@tracked modalTitle;
	@tracked modalText;
	@tracked showModal = false;

	transitions = {
		[states.IDLE]: {
			[actions.START_EDIT]: states.EDIT_LOCATION,
			[actions.UPDATE_CROWD_SIZE]: states.LOADING
		},
		[states.EDIT_LOCATION]: {
			[actions.END_EDIT]: states.IDLE
		},
		[states.LOADING]: {
			[actions.END_LOADING]: states.IDLE,
			[actions.REJECT_ACTION]: states.IDLE
		}
	};

	constructor() {
		super(...arguments);
		this.initMachine();
	}

	[actions.END_EDIT](data) {
		this.args.refresh(data);
	}

	[actions.UPDATE_CROWD_SIZE](data) {
		const method = data > 0 ? 'incrementCrowd' : 'decrementCrowd';
		return this.locationsService[method](this.args.location.id)
			.then((location) => {
				this.store.findRecord('location', location.id)
					.then((loc) => loc.crowdSize = location.crowdSize);
				location.yelpDetails = this.args.location.yelpDetails;
				this.args.refresh(location);
				return this.dispatch(actions.END_LOADING);
			}).catch(({ errors }) => this.dispatch(actions.REJECT_ACTION, errors));
	}

	[actions.REJECT_ACTION](data) {
		this.modalText = data.toString();
		this.modalTitle = "Error!";
		this.showModal = true;
		return console.error(data);
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
	increment() {
		return this.dispatch(actions.UPDATE_CROWD_SIZE, 1);
	}

	@action
	decrement() {
		return this.dispatch(actions.UPDATE_CROWD_SIZE, -1);
	}

}
