import Controller from '@ember/controller';
import { action } from '@ember/object';

export default class AdminLocationsController extends Controller {
	@action
	clickAction(business) {
		return this.transitionToRoute('locations.location', business);
	}
}
