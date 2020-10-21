import Controller from '@ember/controller';
import { action } from '@ember/object'

export default class AccountMeController extends Controller {
	@action
	clickAction(location) {
		return this.transitionToRoute('locations.location', location);
	}
}
