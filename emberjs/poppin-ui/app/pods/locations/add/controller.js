import Controller from '@ember/controller';
import { action } from '@ember/object';

export default class AdminLocationsAddController extends Controller {
	@action
	redirectToLocation(location) {
		this.transitionToRoute('locations.location', location);
	}
}
