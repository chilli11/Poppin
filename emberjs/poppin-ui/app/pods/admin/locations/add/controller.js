import Controller from '@ember/controller';
import { action } from '@ember/object';

export default class AdminLocationsAddController extends Controller {
	@action
	redirectToLocation(location_id) {
		this.transitionToRoute('admin.locations.location', { location_id });
	}
}
