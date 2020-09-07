import Controller from '@ember/controller';
import { action } from '@ember/object';

export default class AdminLocationsLocationController extends Controller {
	@action
	refreshRoute(location) {
		this.transitionToRoute('locations.location', location);
	}
}
