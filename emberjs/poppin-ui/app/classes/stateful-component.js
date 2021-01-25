import Component from '@glimmer/component';
import { Promise } from 'rsvp';
import { tracked } from '@glimmer/tracking';

export const transitionsErrorMessage = 'transitions is non an object';
export const currentStateErrorMessage = 'machineState is not a string';

export default class extends Component {
	scrollToTop = false;
	callStack = [];

	@tracked machineState = 'IDLE';

	/**
	 * Machine start up, must be run manually in the constructor
	 * after any component specific logic has been run.
	 * Sets up transitions and throws errors if no transitions are given,
	 * if the `transitions` prop is not an object, r if the `machineState`
	 * is not a string.
	 * 
	 * Adds '_INIT' to the call stack.
	 */
	initMachine() {
		if (this.transitions) {
			const { transitions, namespace, machineState } = this;
			if (!transitions || typeof transitions !== 'object') {
				console.error(transitionsErrorMessage, namespace);
				return;
			}
			
			if (!machineState || typeof machineState !== 'string') {
				console.error(currentStateErrorMessage, namespace);
			}

			if (this.init && typeof this.init == 'function') {
				this.init();
			}
			this.callStack.push({ machineState: machineState, action: '_INIT_' });
		} else {
			console.error('No model provided');
		}
	}

	/**
	 * Dispatches action through the machine using transitions config.
	 * Multiple levels of error handling.
	 * @param {String} action
	 * @param {*} [data]
	 * @param {Boolean} [scroll] - override for `scrollToTop`
	 */
	dispatch(action, data, scroll) {
		try {
			if (!this) {
				throw new Error('You are lost context when called "dispatch"');
			}
			const { machineState, transitions, callStack, namespace } = this;

			const scrollToTop = typeof scroll === 'undefined' ? this.scrollToTop : scroll;
			const trasnsitionList = transitions[machineState];

			if (scroll && scroll !== true) {
				console.error('Multiple params should be passed as an object, not arguments');
				console.error(...arguments);
			}

			if (!transitions || typeof transitions !== 'object') {
				throw transitionsErrorMessage;
			}

			if (!machineState || typeof machineState !== 'string') {
				throw currentStateErrorMessage;
			}

			if (!transitions[machineState]) {
				throw `${machineState} machineState is not defined in transitions list`;
			}

			const prevAction = callStack.length > 0 ? callStack[callStack.length - 1].action : null;
			callStack.push({ machineState, action, data, namespace });

			const transtionState = trasnsitionList[action];
			if (transtionState) {
				this.machineState = transtionState;
			} else {
				console.error(`Cannot set machineState to ${machineState}. Check transitions:
				* previous action: ${prevAction} => ${machineState}
				* current action: ${action};
				* available actions for ${machineState}: `, trasnsitionList);
			}

			if (scrollToTop) {
				window.scrollTo({
					top: 0,
					left: 0,
					behavior: 'smooth'
				});
			}

			if (typeof this[action] === 'function') {
				if (!transtionState) {
					console.error(`${action} cannot be called from ${machineState} state.`);
					return Promise.resolve();
				}
				return this[action](data);
			}
			return Promise.resolve({ msg: `Action ${action} not implemented` });
		} catch (error) {
			return console.error(error);
		}
	}
}